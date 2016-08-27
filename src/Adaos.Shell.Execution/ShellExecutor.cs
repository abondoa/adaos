using Adaos.Common.Extenders;
using Adaos.Shell.Interface;
using Adaos.Shell.Interface.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adaos.Shell.Execution
{
    class ShellExecutor : IShellExecutor
    {
        private static readonly IEnumerable<IArgument>[] NoArguments = new IEnumerable<IArgument>[] { new IArgument[0] };
        private ErrorHandler _handleError;

        public virtual IEnumerable<IArgument> Execute(IExecutionSequence prog, IVirtualMachine virtualMachine)
        {
            return Execute(prog, NoArguments, virtualMachine);
        }

        public IEnumerable<IArgument> Execute(IExecutionSequence prog, IEnumerable<IArgument>[] args, IVirtualMachine virtualMachine)
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
                return InternExecute(prog, args, virtualMachine).ToArray();
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


        private IEnumerable<IArgument> HandleArguments(IEnumerable<IArgument> args, IVirtualMachine virtualMachine)
        {
            VirtualMachine vm = null;
            foreach (var arg in args)
            {
                if (arg.ToExecute)
                {
                    if (vm == null)
                    {
                        vm = new VirtualMachine(virtualMachine);
                        vm.Parser.ScannerTable = virtualMachine.Parser.ScannerTable.Copy();
                    }
                    IEnumerable<IArgument> results;
                    try
                    {
                        if (arg is IArgumentExecutable)
                            results = InternExecute((arg as IArgumentExecutable).ExecutionSequence, new[] { new IArgument[0] },vm).ToArray();
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


        internal IEnumerable<IArgument> InternExecute(IExecutionSequence prog, IEnumerable<IArgument>[] args, IVirtualMachine virtualMachine)
        {
            IEnumerable<IEnumerable<IArgument>> result = args;

            foreach (IExecution comm in prog.Executions)
            {
                if (!comm.IsPipeRecipient())
                {
                    foreach (var temp in result)
                        temp.ToArray(); //Execute previous before trying to resolve (maybe a new environment has been loaded just before)
                }

                var toExec = virtualMachine.Resolver.Resolve(comm, virtualMachine.EnvironmentContainer.LoadedEnvironments);

                var commandlineArguments = new IEnumerable<IArgument>[] { HandleArguments(comm.Arguments, virtualMachine).ToArray() };

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

    }
}
