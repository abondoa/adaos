using System.Collections.Generic;
using Adaos.Shell.Interface;
using Adaos.Shell.Interface.Exceptions;

namespace Adaos.Shell.SyntaxAnalysis.ASTs
{
    /// <summary>
    /// An abstract class representing a <see cref="ExecutionSequence"/> of the Adaos AST.
    /// The ProgramSequence is the root node of the AST, and its children are a sequence of commands.
    /// </summary>
    public abstract class ExecutionSequence : AST, IExecutionSequence
    {
        /// <summary>
        /// Enumerate the commands of the ProgramSequence.
        /// </summary>
        public abstract IEnumerable<Execution> Executions { get; }

        /// <summary>
        /// The default constructor for the ProgramSequence.
        /// </summary>
        protected ExecutionSequence()
        {
            Errors = new AdaosException[0];
        }

        /// <summary>
        /// A constructor for the ProgramSequence.
        /// </summary>
        /// <param name="position">The position of the first character in the source code.</param>
        protected ExecutionSequence(int position) : base(position)
        {
            Errors = new AdaosException[0];
        }

        /// <summary>
        /// Enumerate the <see cref="IExecution"/> nodes of the programsequence nodes.
        /// </summary>
        IEnumerable<IExecution> IExecutionSequence<IExecution, AdaosException>.Executions
        {
            get { return Executions; }
        }

        /// <summary>
        /// Enumerate the errors while executing a program sequence.
        /// </summary>
        public IEnumerable<AdaosException> Errors
        {
            get;
            internal set;
        }
    }
}
