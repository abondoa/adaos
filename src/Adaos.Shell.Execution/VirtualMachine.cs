using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adaos.Shell.Interface;
using Adaos.Shell.Interface.Exceptions;
using Adaos.Shell.SyntaxAnalysis.Parsing;
using Adaos.Shell.SyntaxAnalysis;
using Adaos.Shell.SyntaxAnalysis.ASTs;
using Adaos.Shell.SyntaxAnalysis.Exceptions;
using System.IO;
using Adaos.Shell.Execution.Exceptions;
using Adaos.Shell.Core;
using Adaos.Shell.Core.Extenders;
using Adaos.Common.Extenders;
using Adaos.Shell.Library.Standard;

namespace Adaos.Shell.Execution
{
    public class VirtualMachine : IVirtualMachine
    {
        private IShellParser _parser;
        private StreamWriter _output;
        private StreamWriter _log;
        private IResolver _resolver;
        private IModuleManager _moduleManager;
        private Adaos.Shell.Interface.ErrorHandler _handleError;
        private IEnvironment _rootEnvironment;
        private EnvironmentContainer _loadedEnvironments;
        private EnvironmentContainer _unloadedEnvironments;

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
            _loadedEnvironments = new EnvironmentContainer();
            _unloadedEnvironments = new EnvironmentContainer();
            _rootEnvironment = new Environments.RootEnvironment();
            _rootEnvironment.AddEnvironments(environments);
            _rootEnvironment.AddEnvironments(
                new Environments.SystemEnvironment(),
                new StandardEnvironment(this));
            foreach(var env in _rootEnvironment.ChildEnvironments.Select(x => x.FamilyEnvironments()).Flatten())
            {
                _loadedEnvironments.Add(env.EnvironmentNames.ToArray(), env);
            }
            _parser = new Parser();
            _resolver = new Resolver();
        }

        public virtual void Execute(string command)
        {
            try
            {
                InternExecute(command).ToArray();
            }
            catch (ExitTerminalException)
            {
                throw;
            }
            catch (AdaosException e)
            {
                HandleError(e);
            }
            /*catch (Exception e)
            {
                HandleError(new UndefinedException(-1,"Unknown", e));
            }*/
        }

        public virtual IEnumerable<IArgument> Execute(IProgramSequence prog)
        {
            if (prog.Errors.Count() > 0)
            {
                foreach (var error in prog.Errors)
                {
                    HandleError(error);
                }
                return new IArgument[0];
            }
            try
            {
                return InternExecute(prog).ToArray();
            }
            catch (ExitTerminalException)
            {
                throw;
            }
            catch (AdaosException e)
            {
                HandleError(e);
                return new IArgument[0];
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
                foreach (var env in LoadedEnvironments)
                {
                    yield return env;
                }
            }
            //private set
            //{
            //    if (value != null)
            //    {
            //        IEnvironment last = null;
            //        EnvironmentContainer tempContainer = new EnvironmentContainer();
            //        foreach (var item in value.Then(LoadedEnvironments.Where(x => x.Name.Equals("system"))).OrderBy((x) => x.Name))
            //        {
            //            if (last != null && item.Name.ToLower().Equals(last.Name.ToLower()))
            //            {
            //                throw new VMException(-1,"Name conflict between environments on the name: " + item.Name);
            //            }
            //            last = item;
            //            tempContainer.Add(item.)
            //        }
            //        LoadedEnvironments = value.Union(LoadedEnvironments.Where(x => x.Name.Equals("system"))).ToList();
            //    }
            //}
        }

        public Adaos.Shell.Interface.ErrorHandler HandleError
        {
            get
            {
                if (_handleError == null)
                {
                    _handleError = (AdaosException e) => { throw e; }; 
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
            if (LoadedEnvironments.Contains(environment))
            {
                throw new ArgumentException("This environment is already loaded (" + environment.Name + ")");
            }
            if (LoadedEnvironments.FirstOrDefault(x => x.Name.ToLower().Equals(environment.Name.ToLower())) != null)
            {
                throw new ArgumentException("Environment name conflict on name: " + environment.Name);
            }
            IEnvironmentContext toLoad;
            if ((toLoad = _unloadedEnvironments.FirstOrDefault(x => x.Value == environment).Value) != null)
            {
                _unloadedEnvironments.Remove(toLoad.EnvironmentNames.ToArray());
                _loadedEnvironments.Add(toLoad.EnvironmentNames.ToArray(), toLoad);
            }
            else
            {
                _rootEnvironment.AddEnvironment(environment);
                foreach (var env in environment.FamilyEnvironments())
                {
                    _loadedEnvironments.Add(env.EnvironmentNames.ToArray(), env);
                }
            }
        }

        public void UnloadEnvironment(IEnvironment environment)
        {
            if (!LoadedEnvironments.Contains(environment))
            {
                throw new ArgumentException("This environment is not loaded (" + environment.Name + ")");
            }
            if (environment == LoadedEnvironments.FirstOrDefault(x => x.Name == "system"))
            {
                throw new ArgumentException("Trying to remove system environment, this is not legal!");
            }
            var context = _rootEnvironment.DecendentEnvironments().FirstOrDefault(x => x == environment);
            _loadedEnvironments.Remove(context.EnvironmentNames.ToArray());
            _unloadedEnvironments.Add(context.EnvironmentNames.ToArray(), context);
        }

        #region Properties

        private IEnumerable<IEnvironment> LoadedEnvironments 
        {
            get 
            {
                return _loadedEnvironments.Select(x => x.Value);
            }
        }

        public IEnvironment PrimaryEnvironment
        {
            get
            {
                return LoadedEnvironments.FirstOrDefault();
            }
            set
            {
                throw new NotImplementedException("PrimaryEnvironment");
                if (Environments.Contains(value))
                {
                    //LoadedEnvironments = new List<IEnvironment>() { value }.Then(LoadedEnvironments).Distinct().ToList();
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

        public IModuleManager ModuleManager
        {
            get
            {
                return _moduleManager;
            }
            set
            {
                if (value == null) throw new ArgumentNullException("ModuleManager");
                _moduleManager = value;
            }
        }

        public IResolver Resolver
        {
            get
            {
                return _resolver;
            }
            set
            {
                if (value == null) throw new ArgumentNullException("Resolver");
                _resolver = value;
            }
        }

        #endregion Properties

        public string SuggestCommand(string partialCommand)
        {
            IProgramSequence prog = _parser.Parse(partialCommand);
			var lastCommand = prog.Commands.LastOrDefault();
			if (lastCommand == null)
				return null;
			string qualifiedEnvName = lastCommand.EnvironmentNames.Aggregate (
				(x,y) => x + Parser.ScannerTable.EnvironmentSeparator + y);
            var envs = Environments.Where(y => y.Name.StartsWith(qualifiedEnvName));
            if (envs.FirstOrDefault() != null && envs.Skip(1).FirstOrDefault() == null)
            {
				StringBuilder suggestion = new StringBuilder (envs.First().Name);
				if (envs.First().Name == qualifiedEnvName) 
				{
					var cmds = envs.First ().Commands.Where(x => x.StartsWith(lastCommand.CommandName));
					if (cmds.FirstOrDefault () != null && cmds.Skip (1).FirstOrDefault () == null) 
					{
					    suggestion.Append (_parser.ScannerTable.EnvironmentSeparator + cmds.First ());
					}
				}
                return  suggestion.ToString();
            }
            return null;
        }
    }
}
