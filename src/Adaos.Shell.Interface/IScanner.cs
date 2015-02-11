namespace Adaos.Shell.Interface
{
    /// <summary>
    /// An interface describing the scanner component of Adaos syntax analysis.
    /// </summary>
    /// <typeparam name="TToken">The type of token scanned by the <see cref="IScanner"/>.</typeparam>
    public interface IScanner<TToken>
    {
        /// <summary>
        /// Get the next token in the source string.
        /// </summary>
        /// <returns>A token conforming to the Adaos shell language.</returns>
        TToken Scan();
    }
}
