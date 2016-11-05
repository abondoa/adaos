using System.Collections.Generic;
using Adaos.Shell.Interface.SyntaxAnalysis;
using System.IO;

namespace Adaos.Shell.Interface.Execution
{
    public delegate void ScopeListener();
    /// <summary>
    /// An interface describing a virtual machine used to execute commands of the Adaos language.
    /// </summary>
    public interface IShellExecutor
    {
        /// <summary>
        /// The virtual machine will execute the given parsed execution sequence.
        /// </summary>
        /// <param name="prog">The parse tree/concrete syntax tree to execute.</param>
        IEnumerable<IArgument> Execute(IExecutionSequence prog, IVirtualMachine virtualMachine);

        /// <summary>
        /// The virtual machine will execute the given parsed execution sequence with the arguments piped in the first execution.
        /// </summary>
        /// <param name="prog">The parse tree/concrete syntax tree to execute.</param>
        /// <param name="args">Arguments piped into the first execution.</param>
        /// <returns>Arguments output from the last execution</returns>
        IEnumerable<IArgument> Execute(IExecutionSequence prog, IEnumerable<IArgument>[] args, IVirtualMachine virtualMachine);
        
        /// <summary>
        /// Get or set the errorhandler.
        /// </summary>
        ErrorHandler HandleError { get; set; }

        event ScopeListener ScopeOpened;
        event ScopeListener ScopeClosed;
    }
}

