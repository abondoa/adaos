namespace Adaos.Shell.Interface.SyntaxAnalysis
{
    /// <summary>
    /// An interface describing an argument node in the Adaos AST.
    /// </summary>
    public interface IArgumentExecutable : IArgument
    {
        /// <summary>
        /// Get the name of the argument.
        /// </summary>
        IExecutionSequence ExecutionSequence { get; }
    }
}
