using System.Collections.Generic;
using Adaos.Shell.Interface;
using Adaos.Shell.Interface.Exceptions;
using Adaos.Shell.Interface.SyntaxAnalysis;

namespace Adaos.Shell.SyntaxAnalysis.ASTs
{
    /// <summary>
    /// An abstract class representing a <see cref="ProgramSequence"/> of the Adaos AST.
    /// The ProgramSequence is the root node of the AST, and its children are a sequence of commands.
    /// </summary>
    public abstract class ProgramSequence : AST, IProgramSequence
    {
        /// <summary>
        /// Enumerate the commands of the ProgramSequence.
        /// </summary>
        public abstract IEnumerable<Command> Commands { get; }

        /// <summary>
        /// The default constructor for the ProgramSequence.
        /// </summary>
        protected ProgramSequence() { }

        /// <summary>
        /// A constructor for the ProgramSequence.
        /// </summary>
        /// <param name="position">The position of the first character in the source code.</param>
        protected ProgramSequence(int position) : base(position) { }

        /// <summary>
        /// Enumerate the <see cref="ICommand"/> nodes of the programsequence nodes.
        /// </summary>
        IEnumerable<ICommand> IProgramSequence<ICommand, AdaosException>.Commands
        {
            get { return Commands; }
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
