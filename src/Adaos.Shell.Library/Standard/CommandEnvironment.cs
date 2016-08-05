using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adaos.Shell.Interface;
using System.IO;
using Adaos.Shell.Core.Extenders;
using Adaos.Shell.Core;
using Adaos.Common.Extenders;
using Adaos.Shell.Interface.Exceptions;

namespace Adaos.Shell.Library.Standard
{
    class CommandEnvironment : BaseEnvironment
    {
        virtual protected StreamWriter _output { get; private set;}
        virtual protected IVirtualMachine _vm { get; private set; }

        public override string Name
        {
            get { return "command"; } 
        }

        public CommandEnvironment(StreamWriter output, IVirtualMachine vm = null)
        {
            _output = output;
            _vm = vm;
            Bind(Cmds, "commands/cmds silent/si=false environments/*");
            Bind(Repeat, "repeat", "rep");
        }

        private IEnumerable<IArgument> Cmds(IArgumentValueLookup lookup, params IEnumerable<IArgument>[] args)
        {
            List<IArgument> result = new List<IArgument>();
            bool verbose;
            lookup["silent"].TryParseTo(out verbose);
            verbose = !verbose;
            foreach (var arg in lookup.Lookup["environments"].Then(args.Flatten()))
            {
                IEnvironment env = _vm.EnvironmentContainer.LoadedEnvironments.FirstOrDefault(x => x.QualifiedName (_vm.Parser.ScannerTable.EnvironmentSeparator).ToLower().Equals(arg.Value.ToLower()));
                if (env == null)
                {
                    throw new SemanticException(arg.Position, arg.Value + " is not a loaded environment");
                }
                string toWrite = "";
                foreach (var item in env.Commands)
                {
                    result.Add(new DummyArgument(item));

                    if (verbose)
                    {
                        toWrite += item + " ";
                    }
                }
                if (verbose)
                {
                    if (toWrite.Length > 0)
                    {
                        _output.WriteLine("Commands for " + env.Name + " environment:");
                        _output.WriteLine(toWrite.Substring(0,toWrite.Length-1));
                    }
                    else
                    {
                        _output.WriteLine("No commands for environment: " + env.Name + "");
                    }
                }
            }
            return result;
        }

        private IEnumerable<IArgument> Repeat(IEnumerable<IArgument> args)
        {
            args.VerifyArgumentMinCount(2, x => { throw new SemanticException(-1, "Failure in Repeat command. " +x); });

            var cmd = args.First().Value;
            var res = _vm.Resolver;
            var commandSeq = _vm.Parser.Parse(cmd);
            if (commandSeq == null || commandSeq.Errors == null)
            {
                throw new SemanticException(-1,"The program '" + args.First().Value + "' could not be parsed");
            }
            if (commandSeq.Errors.Count() > 0)
            {
                StringBuilder str = new StringBuilder();
                str.Append("The program '" + args.Skip(1).First().Value + "' could not be parsed. Errors received: ");
                str.Append(commandSeq.Errors.First().Message);
                foreach (var err in commandSeq.Errors.Skip(1))
                {
                    str.Append("; " + err.Message);
                }
                throw new SemanticException(-1, str.ToString());
            }
            IExecution command = commandSeq.Executions.First();
            var executableCommand = res.Resolve(command, _vm.EnvironmentContainer.LoadedEnvironments);
            foreach (var arg in args.Skip(1))
            {
                foreach (var result in executableCommand(new List<IArgument>{arg}))
                {
                    yield return result;
                }
            }
        }

        private void addDependency(Type ident, IList<Type> dependencies)
        {
            if (!dependencies.Contains(ident))
            {
                dependencies.Add(ident);
            }
        }
    }
}
