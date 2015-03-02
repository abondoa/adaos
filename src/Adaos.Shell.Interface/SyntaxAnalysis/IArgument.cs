namespace Adaos.Shell.Interface.SyntaxAnalysis
{
    /// <summary>
    /// An interface describing an argument node in the Adaos AST.
    /// </summary>
    public interface IArgument
    {
        /// <summary>
        /// Get the name of the argument.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Get the value of the argument - as a string.
        /// </summary>
        string Value { get; }

        /// <summary>
        /// Get whether this argument is to be executed.
        /// </summary>
        bool ToExecute { get; }

        /// <summary>
        /// Get the position of the argument in the source string.
        /// </summary>
        int Position { get; }

        /// <summary>
        /// Get whether the argument has a name.
        /// </summary>
        bool HasName { get; }
    }
}
