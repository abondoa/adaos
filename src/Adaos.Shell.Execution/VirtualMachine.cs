using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adaos.Shell.Interface;
using Adaos.Shell.Interface.SyntaxAnalysis;
using Adaos.Shell.Interface.Execution;
using Adaos.Shell.Interface.Exceptions;
using Adaos.Shell.SyntaxAnalysis.Parsing;
using System.IO;
using Adaos.Shell.ModuleHandling;

namespace Adaos.Shell.Execution
{
    public class VirtualMachine : IVirtualMachine
    {
        private IShellParser _parser;
        private StreamWriter _output;
        private StreamWriter _log;
        private IResolver _resolver;
        private IModuleManager _moduleManager;
        private IEnvironmentContainer _envContainer;
        private IEnumerable<IArgument>[] NoArguments = new IEnumerable<IArgument>[] {new IArgument[0] };
        private IShellExecutor _shellExecutor;

        public VirtualMachine(StreamWriter output, StreamWriter log)
        {
            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }
            if (log == null)
            {
                throw new ArgumentNullException(nameof(log));
            }
            _output = output;
            _log = log;

            _parser = new Parser();
            _resolver = new Resolver();
            _moduleManager = new ModuleManager();
            _shellExecutor = new ShellExecutor();
            _envContainer = new EnvironmentContainer(Library.StandardLibraryContextBuilder.Instance.BuildEnvironments(this));
        }

        public VirtualMachine(StreamWriter output, StreamWriter log, IEnvironmentContainer container)
        {
            if (output == null)
            {
                throw new ArgumentNullException(nameof(output));
            }
            if (log == null)
            {
                throw new ArgumentNullException(nameof(log));
            }
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }
            _output = output;
            _log = log;

            _envContainer = container;
            _parser = new Parser();
            _resolver = new Resolver();
            _moduleManager = new ModuleManager();
            _shellExecutor = new ShellExecutor();
        }

        public VirtualMachine(IVirtualMachine virtualMachine) : this(virtualMachine.Log,virtualMachine.Output, virtualMachine.EnvironmentContainer)
        {
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

        public IEnumerable<IArgument> InternExecute(string command, int initialPosition = 0)
        {
            IExecutionSequence prog = _parser.Parse(command, initialPosition);
            if (prog.Errors.Count() > 0)
            {
                foreach (var error in prog.Errors)
                {
                    HandleError(error);
                }
                return new List<IArgument>();
            }
            return ShellExecutor.Execute(prog,this);
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
                if (value == null) throw new ArgumentNullException(nameof(EnvironmentContainer));
                _envContainer = value;
            }
        }

        public IShellExecutor ShellExecutor
        {
            get
            {
                return _shellExecutor;
            }

            set
            {
                if (value == null) throw new ArgumentNullException(nameof(ShellExecutor));
                _shellExecutor = value;
            }
        }

        public ErrorHandler HandleError
        {
            get
            {
                return ShellExecutor.HandleError;
            }

            set
            {
                ShellExecutor.HandleError = value;
            }
        }

        #endregion Properties

        public string SuggestCommand(string partialCommand)
        {
            IExecutionSequence prog = _parser.Parse(partialCommand);
			var lastCommand = prog.Executions.LastOrDefault();
			if (lastCommand == null)
				return null;
			string qualifiedEnvName = lastCommand.EnvironmentNames.Aggregate (
				(x,y) => x + Parser.ScannerTable.EnvironmentSeparator + y);
            var envs = EnvironmentContainer.EnabledEnvironments.Where(y => y.Name.StartsWith(qualifiedEnvName));
            if (envs.FirstOrDefault() != null && envs.Skip(1).FirstOrDefault() == null)
            {
				StringBuilder suggestion = new StringBuilder (envs.First().Name);
				if (envs.First().Name == qualifiedEnvName) 
				{
					var cmds = envs.First ().Commands.Where(x => x.StartsWith(lastCommand.CommandName));
					if (cmds.FirstOrDefault () != null && cmds.Skip (1).FirstOrDefault () == null) 
					{
					    suggestion.Append(_parser.ScannerTable.EnvironmentSeparator + cmds.First());
					}
				}
                return  suggestion.ToString();
            }
            return null;
        }

        public IEnumerable<IArgument> Execute(IExecutionSequence prog)
        {
            return ShellExecutor.Execute(prog, this);
        }

        public IEnumerable<IArgument> Execute(IExecutionSequence prog, IEnumerable<IArgument>[] args)
        {
            return ShellExecutor.Execute(prog, args, this);
        }
    }
}
