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
        private ErrorHandler _handleError;
        private IEnvironmentContainer _envContainer;

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
            

			_envContainer = new EnvironmentContainer (Library.ContextBuilder.Instance.BuildStandardEnvironment(this).ToEnum<IEnvironment>());
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
                if (!comm.IsPipeRecipient())
                {
                    result.ToArray(); //Execute previous before trying to resolve (maybe a new environment has been loaded just before)
                }

                toExec = _resolver.Resolve(comm, EnvironmentContainer.LoadedEnvironments);

                switch (comm.RelationToPrevious)
                {
                    case CommandRelation.Concatenated:
                        result = result.Then(toExec(HandleArguments(comm.Arguments)));
                        break;
<<<<<<< HEAD
                    case CommandRelation.Piped:
                        result = toExec(HandleArguments(comm.Arguments).Then(result));
=======
                    case CommandRelation.PIPED:
                        result = toExec(HandleArguments(comm.Arguments),result);
>>>>>>> 0b52b58481e0ed747d22016d91c0a8dbedb42bcf
                        break;
                    case CommandRelation.Separated:
                        result = toExec(HandleArguments(comm.Arguments));
                        break;
                }
            }

            foreach (var arg in result)
            {
                yield return arg;
            }
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

        private IEnumerable<IArgument> HandleArguments(IEnumerable<IArgument> args)
        {
            VirtualMachine vm = new VirtualMachine(_output,_log,EnvironmentContainer.LoadedEnvironments.ToArray());
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
        }

        #region Properties

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

        public IEnvironmentContainer EnvironmentContainer
        {
            get
            {
                return _envContainer;
            }
            set
            {
                if (value == null) throw new ArgumentNullException("EnvironmentContainer");
                _envContainer = value;
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
            var envs = EnvironmentContainer.LoadedEnvironments.Where(y => y.Name.StartsWith(qualifiedEnvName));
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
