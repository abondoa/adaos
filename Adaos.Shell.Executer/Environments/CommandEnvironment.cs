using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adaos.Shell.Interface;
using System.IO;
using Adaos.Shell.SyntaxAnalysis.Exceptions;
using Adaos.Shell.SyntaxAnalysis.Parsing;
using Adaos.Shell.Executer.Extenders;
using Adaos.Shell.Core;

namespace Adaos.Shell.Executer.Environments
{
    class CommandEnvironment : Environment
    {
        virtual protected StreamWriter _output { get; private set;}
        virtual protected IVirtualMachine _vm { get; private set; }

        private readonly List<string[]> CommonFlagsWithAlias = new List<string[]>
        { 
            new string[]{"-silent","-si"},
            new string[]{"-verbose","-v"}
        };

        public override string Name
        {
            get { return "command"; } 
        }

        public CommandEnvironment(StreamWriter output, IVirtualMachine vm = null)
        {
            _output = output;
            _vm = vm;
            Bind(Cmds, "commands", "cmds");
            Bind(MakeCommand, "makecommand", "mkcmd");
            Bind(RemoveCommand, "removecommand", "rmcmd");
            Bind(Repeat, "repeat", "rep");
        }

        private IEnumerable<IArgument> Cmds(IEnumerable<IArgument> args)
        {
            List<IArgument> result = new List<IArgument>();
            var flags = args.ParseFlagsWithAlias(CommonFlagsWithAlias.ToList(),false);
            bool verbose = !flags.FirstOrDefault(x => x.Key.Equals("-silent")).Value;
            foreach (var arg in args.Where(x => CommonFlagsWithAlias.FirstOrDefault(y => y.Contains(x.Value)) == null))
            {
                IEnvironment env = _vm.Environments.Select(x => x.FamilyEnvironments()).Aggregate((x,y) => x.Union(y)).FirstOrDefault(x => x.Name.ToLower().Equals(arg.Value.ToLower()));
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

        private IEnumerable<IArgument> MakeCommand(IEnumerable<IArgument> args)
        {
            if (args.Count() != 2)
            {
                throw new SemanticException(-1,"MakeCommand must get exactly two arguments");
            }
            IEnvironment custom = _vm.Environments.FirstOrDefault(x => x.Name.Equals("custom"));
            if (custom == null)
            {
                throw new VMException(-1, "ADAOS VM does not have a custom environment loaded");
            }
            var commandSeq = _vm.Parser.Parse(args.Skip(1).First().Value);
            if (commandSeq == null || commandSeq.Errors == null)
            {
                throw new SemanticException(-1, "Failed to make new command: '" + args.First().Value + "'. The program '" + args.Skip(1).First().Value + "' could not be parsed");
            }
            if (commandSeq.Errors.Count() > 0)
            {
                StringBuilder str = new StringBuilder();
                str.Append("Failed to make new command: '" + args.First().Value + "'. The program '" + args.Skip(1).First().Value + "' could not be parsed. Errors received: ");
                str.Append(commandSeq.Errors.First().Message);
                foreach (var err in commandSeq.Errors.Skip(1))
                {
                    str.Append("; " + err.Message);
                }
                throw new SemanticException(-1, str.ToString());
            }
            var res = new Resolver();
            var listOfDeps = new List<IEnvironmentUniqueIdentifier>();
            foreach (var cmd in commandSeq.Commands)
            {
                try
                {
                    var env = res.GetEnvironmentOf(cmd, _vm.Environments);
                    if (env != null)
                    {
                        addDependency(env.Identifier,listOfDeps);
                    }
                }
                catch (VMException e)
                {
					throw new SemanticException(e.Position + args.Second().Position - 1, "While resolving command '"+cmd.EnvironmentNames.Aggregate ((x,y) => x + _vm.Parser.ScannerTable.EnvironmentSeparator + y)+_vm.Parser.ScannerTable.EnvironmentSeparator+cmd.CommandName+"' the following error was received: "+e.Message);
                }
            }
            ICommand last = commandSeq.Commands.Last();
            Command execCommand = (x) =>
                {
                    DummyCommand command = new DummyCommand(last.CommandName, last.EnvironmentNames, last.Arguments.Then(x), last.Position, last.RelationToPrevious);
                    DummyProgramSequence progSec = new DummyProgramSequence(commandSeq.Commands.Where(y => y != last).Then(new List<ICommand> { command }).ToArray());
                    return (_vm as VirtualMachine).InternExecute(progSec);
                };
            if (custom.Retrieve(args.First().Value) != null)
            {
                throw new SemanticException(args.First().Position,"Trying to make new command with exisiting name: '" + args.First().Value + "'. Please use 'rmcmd " + args.First().Value + "' to remove existing command");
            }
            if (custom is CustomEnvironment)
            {
                (custom as CustomEnvironment).Bind(args.First().Value, execCommand, listOfDeps);
            }
            else
            {
                custom.Bind(execCommand, args.First().Value);
            }
            yield break;
        }

        private IEnumerable<IArgument> RemoveCommand(IEnumerable<IArgument> args)
        {
            IEnvironment custom = _vm.Environments.FirstOrDefault(x => x.Name.Equals("custom"));
            if (custom == null)
            {
                throw new VMException(-1, "ADAOS VM does not have a custom environment loaded");
            }
            if(custom.AllowUnbinding)
            {
                foreach (var arg in args)
                {
                    custom.UnBind(arg.Value);
                }
            }
            else
            {
                throw new VMException(-1, "Custom environment does not allow unbinding");
            }
            yield break;
        }

        private IEnumerable<IArgument> Repeat(IEnumerable<IArgument> args)
        {
            args.VerifyArgumentMinCount(2, x => { throw new SemanticException(-1, "Failure in Repeat command. " +x); });

            var cmd = args.First().Value;
            var res = new Resolver();
            var commandSeq = (_vm as VirtualMachine).Parser.Parse(cmd);
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
            ICommand command = commandSeq.Commands.First();
            var executableCommand = res.Resolve(command,_vm.Environments);
            foreach (var arg in args.Skip(1))
            {
                foreach (var result in executableCommand(new List<IArgument>{arg}))
                {
                    yield return result;
                }
            }
        }

        private void addDependency(IEnvironmentUniqueIdentifier ident, IList<IEnvironmentUniqueIdentifier> dependencies)
        {
            if (!dependencies.Contains(ident))
            {
                dependencies.Add(ident);
            }
        }
    }
}
