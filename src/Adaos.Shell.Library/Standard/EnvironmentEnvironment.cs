using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adaos.Shell.Interface;
using Adaos.Shell.Interface.SyntaxAnalysis;
using System.IO;
using Adaos.Shell.Core;
using Adaos.Shell.Core.Extenders;
using Adaos.Common.Extenders;
using Adaos.Shell.Interface.Exceptions;
using Adaos.Shell.Interface.Execution;

namespace Adaos.Shell.Library.Standard
{    
    class EnvironmentEnvironment : BaseEnvironment
    {
        virtual protected StreamWriter _output {get; private set;}
        virtual protected IVirtualMachine _vm { get; private set; }

        public override string Name
        {
            get { return "environment"; } 
        }

        public EnvironmentEnvironment(StreamWriter output, IVirtualMachine vm = null)
        {
            _output = output;
            _vm = vm;
            Bind(EnableEnvironment, "enableenvironment", "eenv");
            Bind(UnloadEnvironment, "disableenvironment", "denv");
            Bind(Environments, "environments/envs silent/si=false");
            Bind(PromoteEnvironments, "promoteenvironments/proenv silent/si=false environments/*");
            Bind(DependenciesCommand, "dependencies/deps silent/si=false environments/*");
            Bind(args => UnloadEnvironment(args).Then(EnableEnvironment(args)), "demoteenvironments", "demenv");
            Bind(DrawEnvironmentTree, "drawenviroments/denvs/printenvs");
            Bind(Where, "where");
        }

        private IEnumerable<IArgument> Where(IEnumerable<IArgument>[] arguments)
        {
            foreach(var arg in arguments.Flatten())
            {
                var env = _vm.Resolver.GetEnvironmentOf(_vm.Parser.Parse(arg.Value).Executions.First(), _vm.EnvironmentContainer.EnabledEnvironments);
                yield return new DummyArgument(env.QualifiedName(_vm.Parser.ScannerTable.EnvironmentSeparator));
            }
        }

        private IEnumerable<IArgument> EnableEnvironment(IEnumerable<IArgument> args)
        {
            if (_vm == null)
            {
                throw new SemanticException(-1, "Virtual machine not set - Cannot add environment");
            }
            foreach (var arg in args)
            {
				var toEnable = _vm.EnvironmentContainer.DisabledEnvironments.FirstOrDefault (
					x => x.QualifiedName (_vm.Parser.ScannerTable.EnvironmentSeparator) == arg.Value);
                if (toEnable == null)
                {
                    throw new SemanticException(arg.Position, "No disabled environment found called: " + arg.Value);
                }
                toEnable.IsEnabled = true;
            }
            yield break;
        }

        private IEnumerable<IArgument> UnloadEnvironment(IEnumerable<IArgument> args)
        {
            if (_vm == null)
            {
                throw new SemanticException(-1, "Virtual machine not set - Cannot remove environment");
            }
            foreach (var arg in args)
            {
				var toDisable = _vm.EnvironmentContainer.EnabledEnvironments.FirstOrDefault (
					x => x.QualifiedName (_vm.Parser.ScannerTable.EnvironmentSeparator) == arg.Value);
				if (toDisable == null)
				{
					throw new SemanticException(arg.Position, "No enabled environment found called: " + arg.Value);
				}
                toDisable.IsEnabled = false;
            }
            yield break;
        } 

        private IEnumerable<IArgument> Environments(IArgumentValueLookup lookup, params IEnumerable<IArgument>[] args)
        {
            List<IArgument> result = new List<IArgument>();
            bool verbose;
            lookup["silent"].TryParseTo(out verbose);
            verbose = !verbose;
            if (verbose)
            {
                _output.WriteLine("Loaded environments:");
            }
            Action<IEnvironment> envWriter = null;
            envWriter = (env) =>
            {
                string name = env.AsContext().QualifiedName(_vm.Parser.ScannerTable.EnvironmentSeparator);
                result.Add(new DummyArgument(name));

                if (verbose)
                {
                    _output.WriteLine(name);
                }
            };
            foreach (var env in _vm.EnvironmentContainer.EnabledEnvironments)
            {
                envWriter(env);
            }
            if (verbose && _vm.EnvironmentContainer.DisabledEnvironments.Any())
            {
                _output.WriteLine("Additional environments:");
                foreach (var item in _vm.EnvironmentContainer.DisabledEnvironments)
                {
                    if (verbose)
                    {
                        _output.WriteLine(item.QualifiedName(_vm.Parser.ScannerTable.EnvironmentSeparator));
                    }
                }
            }
            return result;
        }

        private IEnumerable<IArgument> DependenciesCommand(IArgumentValueLookup lookup, params IEnumerable<IArgument>[] args)
        {
            List<IArgument> result = new List<IArgument>();
            bool verbose;
            lookup["silent"].TryParseTo(out verbose);
            verbose = !verbose;
            foreach (var arg in lookup.Lookup["environments"].Then(args.Flatten()))
            {
                IEnvironment env = _vm.EnvironmentContainer.EnabledEnvironments.FirstOrDefault(x => x.Name.ToLower().Equals(arg.Value.ToLower()));
                if (env == null)
                {
                    throw new SemanticException(arg.Position, arg.Value + " is not a loaded environment");
                }
                string toWrite = "";
                foreach (var item in env.Dependencies)
                {
                    result.Add(new DummyArgument(item.ToString()));

                    if (verbose)
                    {
                        toWrite += item.ToString() + " ";
                    }
                }
                if (verbose)
                {
                    if (toWrite.Length > 0)
                    {
                        _output.WriteLine("Dependencies for " + env.QualifiedName(_vm.Parser.ScannerTable.EnvironmentSeparator) + " environment:");
                        _output.WriteLine(toWrite.Substring(0, toWrite.Length - 1));
                    }
                    else
                    {
                        _output.WriteLine("No Dependencies for environment: " + env.QualifiedName(_vm.Parser.ScannerTable.EnvironmentSeparator) + "");
                    }
                }
            }
            return result;
        }

        //Function to set primary environment/order of environments to look through
        private IEnumerable<IArgument> PromoteEnvironments(IArgumentValueLookup lookup, params IEnumerable<IArgument>[] args)
        {
            if (_vm == null)
            {
                throw new SemanticException(-1, "Virtual machine not set - Cannot get/set primary environment.");
            }
            bool verbose;
            lookup["silent"].TryParseTo(out verbose);
            verbose = !verbose;
            var environments = lookup.Lookup["environments"].Then(args.Flatten());
            if (environments.Count() == 0)
            {
                IEnvironmentContext env = _vm.EnvironmentContainer.EnabledEnvironments.FirstOrDefault();
                if (env == null)
                {
                    throw new SemanticException(-1, "No environments loaded - Cannot get primary environment.");
                }
                if (verbose)
                {
                    _output.Write("Current primary environment is: ");
                }
                _output.WriteLine(env.QualifiedName(_vm.Parser.ScannerTable.EnvironmentSeparator));
                yield return new DummyArgument(env.QualifiedName(_vm.Parser.ScannerTable.EnvironmentSeparator));
                yield break;
            }
            else
            {
                foreach (var arg in environments.Reverse())
                {
                    IArgument result = null;
                    var toSet = _vm.EnvironmentContainer.EnabledEnvironments.FirstOrDefault(x => x.Name.ToLower().Equals(arg.Value.ToLower()));
                    if (toSet == null)
                    {
                        if (_vm.EnvironmentContainer.DisabledEnvironments.FirstOrDefault(x => x.Name.ToLower().Equals(arg.Value.ToLower())) != null)
                        {
                            throw new SemanticException(arg.Position, "Environment '" + arg.Value + "' is not loaded. Use 'loadenvironment' to load it");
                        }
                        throw new SemanticException(arg.Position, "No environment found called: " + arg.Value);
                    }
                    try
                    {
                        if (verbose)
                        {
                            IEnvironmentContext env = _vm.EnvironmentContainer.EnabledEnvironments.FirstOrDefault();
                            _output.Write("Former primary environment was: ");
                            _output.WriteLine(env.QualifiedName(_vm.Parser.ScannerTable.EnvironmentSeparator));
                        }
                        _vm.EnvironmentContainer.PromoteEnvironment(toSet);
                        if (verbose)
                        {
                            _output.Write("New primary environment is: ");
                            _output.WriteLine(toSet.QualifiedName(_vm.Parser.ScannerTable.EnvironmentSeparator));
                        }
                        result = new DummyArgument(toSet.QualifiedName(_vm.Parser.ScannerTable.EnvironmentSeparator));
                    }
                    catch (ArgumentException e)
                    {
                        throw new SemanticException(arg.Position, e.Message);
                    }
                    yield return result;
                }
            }
            yield break;
        }

        private IEnumerable<IArgument> DrawEnvironmentTree(IArgumentValueLookup args, params IEnumerable<IArgument>[] arguments)
        {

            yield break;
        } 
    }
}
