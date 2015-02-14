namespace Adaos.Shell.SyntaxAnalysis.ASTs
{
    /// <summary>
    /// An abstract class representing the name of a command in the Adaos AST.
    /// </summary>
    public abstract class CommandName : AST
    {
        /// <summary>
        /// The default constructor for the CommandName.
        /// </summary>
        protected CommandName() { }

        /// <summary>
        /// A constructor for the CommandName.
        /// </summary>
        /// <param name="position">The position of the first character of the command name in the source string.</param>
        protected CommandName(int position) : base(position) { }
        
    }
}
