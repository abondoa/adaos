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
        private ErrorHandler _handleError;
        private IEnvironmentContainer _envContainer;
        private IEnumerable<IArgument>[] NoArguments = new IEnumerable<IArgument>[] {new IArgument[0] };

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

            _envContainer = new EnvironmentContainer(Library.ContextBuilder.Instance.BuildStandardEnvironment(this).ToEnum<IEnvironment>());
            _parser = new Parser();
            _resolver = new Resolver();
            _moduleManager = new ModuleManager(this);
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
            _moduleManager = new ModuleManager(this);
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

        public virtual IEnumerable<IArgument> Execute(IExecutionSequence prog)
        {
            return Execute(prog, NoArguments);
        }

        public IEnumerable<IArgument> Execute(IExecutionSequence prog, IEnumerable<IArgument>[] args)
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
                return InternExecute(prog,args).ToArray();
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
            return InternExecute(prog, NoArguments);
        }

        internal IEnumerable<IArgument> InternExecute(IExecutionSequence prog, IEnumerable<IArgument>[] args)
        {
            IEnumerable<IEnumerable<IArgument>> result = args;

            foreach (IExecution comm in prog.Executions)
            {
                Adaos.Shell.Interface.Command toExec = null;
                if (!comm.IsPipeRecipient())
                {
                    foreach(var temp in result)
                        temp.ToArray(); //Execute previous before trying to resolve (maybe a new environment has been loaded just before)
                }

                toExec = _resolver.Resolve(comm, EnvironmentContainer.LoadedEnvironments);

                var commandlineArguments = new IEnumerable<IArgument>[] { HandleArguments(comm.Arguments) };

                switch (comm.RelationToPrevious)
                {
                    case CommandRelation.Concatenated:
                        result = result.Then(toExec(commandlineArguments));
                        break;
                    case CommandRelation.Piped:
                        var output = toExec(commandlineArguments.Then(result).ToArray());
                        result = new IEnumerable<IArgument>[] { output };
                        break;
                    case CommandRelation.Separated:
                        output = toExec(commandlineArguments);
                        result = new IEnumerable<IArgument>[] { output };
                        break;
                }
            }

            foreach (var arg in result.First())
            {
                yield return arg;
            }
        }

        public ErrorHandler HandleError
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
            VirtualMachine vm = null;
            foreach (var arg in args)
            {
                if (arg.ToExecute)
                {
                    if(vm == null)
                    {
                        vm = new VirtualMachine(_output, _log, EnvironmentContainer);
                        vm.Parser.ScannerTable = Parser.ScannerTable.Copy();
                    }
                    IEnumerable<IArgument> results;
                    try
                    {
                        if (arg is ArgumentExecutable)
                            results = vm.InternExecute((arg as ArgumentExecutable).ExecutionSequence, new[] { new IArgument[0] }).ToArray();
                        else
                            results = vm.InternExecute(arg.Value, arg.Position - 1).ToArray();
                    }
                    catch (ExitTerminalException)
                    {
                        results = new IArgument[0];
                    }
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
            IExecutionSequence prog = _parser.Parse(partialCommand);
			var lastCommand = prog.Executions.LastOrDefault();
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
