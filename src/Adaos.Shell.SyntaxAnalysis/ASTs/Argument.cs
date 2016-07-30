using Adaos.Shell.Interface;

namespace Adaos.Shell.SyntaxAnalysis.ASTs
{
    /// <summary>
    /// An abstract class defining an <see cref="IArgument"/> node in the Adaos AST.
    /// </summary>
    public abstract class Argument : AST, IArgument
    {
        /// <summary>
        /// A constructor the Argument class.
        /// </summary>
        /// <param name="position">The position of the first character of the argument.</param>
        /// <param name="execute">Whether the argument is executable (i.e. it is a sub-command)</param>
        /// <param name="wordName">The word representing the argument name, if any.</param>
        protected Argument(int position, bool execute, Word wordName = null)
            : base(position)
        {
            ToExecute = execute;
            WordName = wordName;
        }

        /// <summary>
        /// A constructor the Argument class.
        /// </summary>
        /// <param name="execute">Whether the argument is executable (i.e. it is a sub-command)</param>
        /// <param name="wordName">The word representing the argument name, if any.</param>
        protected Argument(bool execute, Word wordName = null)
        {
            ToExecute = execute;
            WordName = wordName;
        }

        /// <summary>
        /// Get the name of the argument.
        /// If the WordName is null, the Argument is unnamed.
        /// </summary>
        public Word WordName { get; private set; }

        /// <summary>
        /// Get the value of the argument.
        /// </summary>
        public abstract string Value { get; }

        /// <summary>
        /// Get whether this argument is executable.
        /// </summary>
        public virtual bool ToExecute { get; protected set; }

        /// <summary>
        /// Get the name of the argument as a string.
        /// Null if the argument is unnamed.
        /// </summary>
        public virtual string Name
        {
            get
            {
                return WordName?.Spelling;
            }
        }

        /// <summary>
        /// Get whether the argument has a name.
        /// </summary>
        public virtual bool HasName
        {
            get { return WordName != null; }
        }
    }
}
