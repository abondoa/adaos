using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adaos.Shell.Interface;
using System.IO;
using Adaos.Shell.Core;
using Adaos.Shell.Core.Extenders;
using Adaos.Common.Extenders;
using Adaos.Shell.Interface.Exceptions;

namespace Adaos.Shell.Library.Standard
{
    class EnvironmentEnvironment : BaseEnvironment
    {
        virtual protected StreamWriter _output {get; private set;}
        virtual protected List<IEnvironment> _unloadedEnvironments { get; private set; }
        virtual protected IVirtualMachine _vm { get; private set; }
        private readonly List<string[]> CommonFlagsWithAlias = new List<string[]>
        { 
            new string[]{"-silent","-si"},
            new string[]{"-verbose","-v"}
        };

        public override string Name
        {
            get { return "environment"; } 
        }

        public EnvironmentEnvironment(StreamWriter output, IVirtualMachine vm = null)
        {
            _output = output;
            _unloadedEnvironments = new List<IEnvironment>();
            _vm = vm;
            Bind(LoadEnvironment, "loadenvironment", "lenv");
            Bind(UnloadEnvironment, "unloadenvironment", "uenv");
            Bind(Environments, "environments", "envs");
            Bind(PromoteEnvironments, "promoteenvironments", "penv");
            Bind(DependenciesCommand, "dependencies", "deps");
            Bind(args => UnloadEnvironment(args).Then(LoadEnvironment(args)), "demoteenvironments", "denv");
        }

        private IEnumerable<IArgument> LoadEnvironment(IEnumerable<IArgument> args)
        {
            if (_vm == null)
            {
                throw new SemanticException(-1, "Virtual machine not set - Cannot add environment");
            }
            foreach (var arg in args)
            {
                var toAdd = _unloadedEnvironments.FirstOrDefault(x => x.Name.ToLower().Equals(arg.Value.ToLower()));
                if (toAdd == null)
                {
                    throw new SemanticException(arg.Position, "No unloaded environment found called: " + arg.Value);
                }
                _vm.LoadEnvironment(toAdd);
                _unloadedEnvironments.Remove(toAdd);
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
                var toRemove = _vm.Environments.Select(x => x.FamilyEnvironments()).Aggregate((x,y) => x.Union(y)).FirstOrDefault(x => x.Name.ToLower().Equals(arg.Value.ToLower()));
                if (toRemove == null)
                {
                    throw new SemanticException(arg.Position, "No loaded environment found called: " + arg.Value);
                }
                try
                {
                    _vm.UnloadEnvironment(toRemove);
                    _unloadedEnvironments.Add(toRemove);
                }
                catch (ArgumentException e)
                {
                    throw new SemanticException(arg.Position, e.Message);
                }
            }
            yield break;
        } 

        private IEnumerable<IArgument> Environments(IEnumerable<IArgument> args)
        {
            List<IArgument> result = new List<IArgument>();
            var flags = args.ParseFlagsWithAlias(CommonFlagsWithAlias.ToList());
            bool verbose = !flags.FirstOrDefault(x => x.Key.Equals("-silent")).Value;
            if (verbose)
            {
                _output.WriteLine("Loaded environments:");
            }
            Action<IEnvironment> envWriter = null;
            envWriter = (env) =>
            {
                string name = env.ToContext().QualifiedName(_vm.Parser.ScannerTable.EnvironmentSeparator);
                result.Add(new DummyArgument(name));

                if (verbose)
                {
                    _output.WriteLine(name);
                }
            };
            foreach (var env in _vm.Environments)
            {
                envWriter(env);
            }
            if (verbose && _unloadedEnvironments.Count > 0)
            {
                _output.WriteLine("Additional environments:");
                foreach (var item in _unloadedEnvironments)
                {
                    if (verbose)
                    {
                        _output.WriteLine(item.Name);
                    }
                }
            }
            return result;
        }

        private IEnumerable<IArgument> DependenciesCommand(IEnumerable<IArgument> args)
        {
            List<IArgument> result = new List<IArgument>();
            var flags = args.ParseFlagsWithAlias(CommonFlagsWithAlias.ToList(), false);
            bool verbose = !flags.FirstOrDefault(x => x.Key.Equals("-silent")).Value;
            foreach (var arg in args.Where(x => CommonFlagsWithAlias.FirstOrDefault(y => y.Contains(x.Value)) == null))
            {
                IEnvironment env = _vm.Environments.FirstOrDefault(x => x.Name.ToLower().Equals(arg.Value.ToLower()));
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
                        _output.WriteLine("Dependencies for " + env.Name + " environment:");
                        _output.WriteLine(toWrite.Substring(0, toWrite.Length - 1));
                    }
                    else
                    {
                        _output.WriteLine("No Dependencies for environment: " + env.Name + "");
                    }
                }
            }
            return result;
        }

        //Function to set primary environment/order of environments to look through
        private IEnumerable<IArgument> PromoteEnvironments(IEnumerable<IArgument> args)
        {
            if (_vm == null)
            {
                throw new SemanticException(-1, "Virtual machine not set - Cannot get/set primary environment");
            }
            var flags = args.ParseFlagsWithAlias(CommonFlagsWithAlias.ToList(), false).ToDictionary(x => x.Key, x => x.Value);
            bool verbose = flags["-verbose"];
            bool silent = flags["-silent"];
            args = args.Where(x => CommonFlagsWithAlias.FirstOrDefault(y => y.Contains(x.Value)) == null);
            if (args.Count() == 0)
            {
                IEnvironment env = _vm.PrimaryEnvironment;
                if (verbose)
                {
                    _output.Write("Current primary environment is: ");
                }
                _output.WriteLine(env.Name);
                yield return new DummyArgument(env.Name);
                yield break;
            }
            else
            {
                foreach (var arg in args.Reverse())
                {
                    IArgument result = null;
                    var toSet = _vm.Environments.FirstOrDefault(x => x.Name.ToLower().Equals(arg.Value.ToLower()));
                    if (toSet == null)
                    {
                        if (_unloadedEnvironments.FirstOrDefault(x => x.Name.ToLower().Equals(arg.Value.ToLower())) != null)
                        {
                            throw new SemanticException(arg.Position, "Environment '" + arg.Value + "' is not loaded. Use 'loadenvironment' to load it");
                        }
                        throw new SemanticException(arg.Position, "No environment found called: " + arg.Value);
                    }
                    try
                    {
                        if (verbose)
                        {
                            IEnvironment env = _vm.PrimaryEnvironment;
                            _output.Write("Former primary environment was: ");
                            _output.WriteLine(env.Name);
                        }
                        _vm.PrimaryEnvironment = toSet;
                        if (verbose)
                        {
                            _output.Write("New primary environment is: ");
                            _output.WriteLine(toSet.Name);
                        }
                        result = new DummyArgument(toSet.Name);
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

        internal void AddAdditionalEnvironments(IEnumerable<IEnvironment> envs)
        {
            foreach (var env in envs)
            {
                if (_unloadedEnvironments.Exists(x => x.Name == env.Name))
                {
                    throw new ArgumentException("Environment with name: '" + env.Name + "' already available.");
                }
            }

            _unloadedEnvironments.AddRange(envs);
        }

        internal void RemoveAdditionalEnvironments(IEnumerable<IEnvironment> envs)
        {
            foreach (var env in envs)
            {
                if (!_unloadedEnvironments.Exists(x => x.Name == env.Name))
                {
                    if (_vm.Environments.Select(x => x.Name).Contains(env.Name))
                    {
                        throw new ArgumentException("Environment with name: '" + env.Name + "' loaded in virtual machine. Unload with command: '" + Name + ".unloadenvironment " + env.Name + "'");
                    }
                    throw new ArgumentException("Environment with name: '" + env.Name + "' not available.");
                }
            }

            foreach (var env in envs)
            {
                _unloadedEnvironments.Remove(env);
            }
        }
    }
}
