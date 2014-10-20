using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adaos.Shell.Interface;
using Adaos.Shell.SyntaxAnalysis.Parsing;
using Adaos.Shell.SyntaxAnalysis;
using Adaos.Shell.SyntaxAnalysis.ASTs;
using Adaos.Shell.SyntaxAnalysis.Exceptions;
using Adaos.Shell.Executer.Extenders;
using System.IO;
using Adaos.Shell.Executer.Exceptions;
using Adaos.Shell.Core;

namespace Adaos.Shell.Executer
{
    public class VirtualMachine : IVirtualMachine
    {
        private List<IEnvironment> _environments;
        private IShellParser _parser;
        private StreamWriter _output;
        private StreamWriter _log;
        private Resolver _resolver;

        public VirtualMachine(StreamWriter output, StreamWriter log, params IEnvironment[] environments)
        {
            if (output == null)
            {
                throw new ArgumentNullException("Output");
            }
            if (log == null)
            {
                throw new ArgumentNullException("Log");
            }
            if (environments == null)
            {
                throw new ArgumentNullException("Environments");
            }
            _output = output;
            _log = log;
            _environments = new List<IEnvironment>();
            Environments = environments;
            _environments.Add(new Environments.SystemEnvironment(this));
            _environments.Add(new Environments.StandardEnvironment(this));
            _parser = new Parser();
            _resolver = new Resolver();
        }

        public virtual void Execute(string command)
        {
            try
            {
                InternExecute(command).ToArray();
            }
            catch (ExitShellException)
            {
                throw;
            }
            catch (ShellException e)
            {
                HandleError(e);
            }
            /*catch (Exception e)
            {
                HandleError(new UndefinedException(-1,"Unknown", e));
            }*/
        }

        internal IEnumerable<IArgument> InternExecute(string command, int initialPosition = 0)
        {

            IProgramSequence prog = _parser.Parse(command, initialPosition);
            if (prog.Errors.Count() > 0)
            {
                foreach (var error in prog.Errors)
                {
                    HandleError(error);
                }
                return new List<IArgument>();
            }
            return InternExecute(prog);
            
        }

        internal IEnumerable<IArgument> InternExecute(IProgramSequence prog)
        {
            IEnumerable<IArgument> result;
            result = new IArgument[0];

            foreach (ICommand comm in prog.Commands)
            {
                Adaos.Shell.Interface.Command toExec = null;
                if (!comm.IsPiped())
                {
                    result.ToArray(); //Execute previous before trying to resolve (maybe a new environment has been loaded just before)
                }

                toExec = _resolver.Resolve(comm, Environments);

                switch (comm.RelationToPrevious)
                {
                    case CommandRelation.CONCATENATED:
                        result = result.Then(toExec(HandleArguments(comm.Arguments)));
                        break;
                    case CommandRelation.PIPED:
                        result = toExec(HandleArguments(comm.Arguments).Then(result));
                        break;
                    case CommandRelation.SEPARATED:
                        result = toExec(HandleArguments(comm.Arguments));
                        break;
                }
            }

            foreach (var arg in result)
            {
                yield return arg;
            }
        }

        public IEnumerable<IEnvironment> Environments
        {
            get
            {
                foreach (var env in _environments)
                {
                    yield return env;
                }
            }
            private set
            {
                if (value != null)
                {
                    value.OrderBy((x) => x.Name);
                    IEnvironment last = null;
                    foreach (var item in value)
                    {
                        if (last != null && item.Name.ToLower().Equals(last.Name.ToLower()))
                        {
                            throw new VMException(-1,"Name conflict between environments on the name: " + item.Name);
                        }
                        last = item;
                    }
                    _environments = value.Union(_environments.Where(x => x.Name.Equals("system") || x.Name.Equals("redo"))).ToList();
                }
            }
        }

        private Adaos.Shell.Interface.ErrorHandler _handleError;

        public Adaos.Shell.Interface.ErrorHandler HandleError
        {
            get
            {
                if (_handleError == null)
                {
                    _handleError = (ShellException e) => { throw e; }; 
                }
                return _handleError;
            }
            set
            {
                _handleError = value;
            }
        }

        public IEnvironment GetEnv(string name)
        {
            foreach (var env in Environments)
            {
                if (env.Name.Equals(name))
                {
                    return env;
                }
            }

            throw new ArgumentException("No environment is named: '" + name + "'");
        }

        private IEnumerable<IArgument> HandleArguments(IEnumerable<IArgument> args)
        {
            VirtualMachine vm = new VirtualMachine(_output,_log,Environments.ToArray());
            vm.Parser.ScannerTable = Parser.ScannerTable.Copy();
            foreach (var arg in args)
            {
                if (arg.ToExecute)
                {
                    IEnumerable<IArgument> results = vm.InternExecute(arg.Value,arg.Position-1);
                    foreach (var res in results)
                    {
                        yield return res;
                    }
                }
                else
                {
                    yield return arg;
                }
            }
            vm = null;

            yield break;
        }

        public void LoadEnvironment(IEnvironment environment)
        {
            if (_environments.Contains(environment))
            {
                throw new ArgumentException("This environment is already loaded (" + environment.Name + ")");
            }
            if (_environments.FirstOrDefault(x => x.Name.ToLower().Equals(environment.Name.ToLower())) != null)
            {
                throw new ArgumentException("Environment name conflict on name: " + environment.Name);
            }
            _environments.Add(environment);
        }

        public void UnloadEnvironment(IEnvironment environment)
        {
            if (!_environments.Contains(environment))
            {
                throw new ArgumentException("This environment is not loaded (" + environment.Name + ")");
            }
            if (environment == _environments.FirstOrDefault(x => x.Name == "system"))
            {
                throw new ArgumentException("Trying to remove system environment, this is not legal!");
            }
            _environments.Remove(environment);
        }

        public IEnvironment PrimaryEnvironment
        {
            get
            {
                return _environments.FirstOrDefault();
            }
            set
            {
                if (Environments.Contains(value))
                {
                    _environments = new List<IEnvironment>() { value }.Then(_environments).Distinct().ToList();
                }
                else
                {
                    throw new ArgumentException("This environment is not loaded (" + value.Name + ")");
                }
            }
        }

        public IShellParser Parser
        {
            get
            {
                return _parser;
            }
            set
            {
                if (value == null) throw new ArgumentNullException("Parser");
                _parser = value;
            }
        }


        public StreamWriter Output
        {
            get
            {
                return _output;
            }
            set
            {
                if (value == null) throw new ArgumentNullException("Output");
                _output = value;
            }
        }

        public StreamWriter Log
        {
            get
            {
                return _log;
            }
            set
            {
                if (value == null) throw new ArgumentNullException("Log");
                _log = value;
            }
        }


        public string SuggestCommand(string partialCommand)
        {
            IProgramSequence prog = _parser.Parse(partialCommand);
            var envs = Environments.Where(y => y.Name.StartsWith(partialCommand));
            if (envs.FirstOrDefault() != null)
            {
                if (envs.Skip(1).FirstOrDefault() == null)
                {
                    return envs.First().Name + Parser.ScannerTable.EnvironmentSeparator;
                }
            }
            return null;
        }
    }
}
