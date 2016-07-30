using System.Collections.Generic;
using Adaos.Shell.Interface;

namespace Adaos.Shell.SyntaxAnalysis.ASTs
{
    /// <summary>
    /// An abstract base class for command nodes in the Adaos AST.
    /// </summary>
    public abstract class ExecutionBase : Execution
    {
        /// <summary>
        /// Get the AST node representing the command name.
        /// </summary>
        public CommandName CommName { get; private set; }

        /// <summary>
        /// Get the AST node representing the sequence of arguments associated with the command.
        /// </summary>
        public ArgumentSequence Args { get; private set; }

        /// <summary>
        /// Get the position of the first character of the command name in the source string.
        /// </summary>
        public override int Position
        {
            get
            {
                return CommName.Position;
            }
        }

        /// <summary>
        /// Get the command name as a string.
        /// </summary>
        public override string CommandName
        {
            get
            {
                return CommName.ToString();
            }
        }

        /// <summary>
        /// Enumerates the arguments from the <see cref="ArgumentSequence"/> AST node.
        /// </summary>
        public override IEnumerable<IArgument> Arguments
        {
            get {
                return Args.Arguments;
            }
        }

        /// <summary>
        /// A constructof for the base command AST node.
        /// </summary>
        /// <param name="commandName">The AST node representing the name of the command.</param>
        /// <param name="arguments">The AST node representing the argument sequence associated with the command.</param>
        protected ExecutionBase(CommandName commandName, ArgumentSequence arguments)
        {
            CommName = commandName;
            Args = arguments;
        }

        /// <summary>
        /// A constructof for the base command AST node.
        /// </summary>
        /// <param name="position">The position of the first character of the command name in the source string.</param>
        /// <param name="commandName">The AST node representing the name of the command.</param>
        /// <param name="arguments">The AST node representing the argument sequence associated with the command.</param>
        protected ExecutionBase(int position, CommandName commandName, ArgumentSequence arguments) : base(position)
        {
            CommName = commandName;
            Args = arguments;
        }
    }
}
