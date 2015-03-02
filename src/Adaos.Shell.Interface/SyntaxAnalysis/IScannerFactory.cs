namespace Adaos.Shell.Interface
{
    /// <summary>
    /// An interface that describes a factory for creating <see cref="IScanner"/> instances, 
    /// used during Adaos syntax analysis.
    /// </summary>
    /// <typeparam name="TToken">The type of token scanned by the <see cref="IScanner"/> instances.</typeparam>
    public interface IScannerFactory<TToken>
    {
        /// <summary>
        /// Create a <see cref="IScanner"/> instance, with a given input string.
        /// </summary>
        /// <param name="input">The source input to the syntax scanner.</param>
        IScanner<TToken> Create(string input);

        /// <summary>
        /// Create a <see cref="IScanner"/> instance, with a given input string.
        /// </summary>
        /// <param name="input">The source input to the syntax scanner.</param>
        /// <param name="extraPosition">Set the initial character position for the syntax scanner. 
        /// This is used if the scanned input string, is a substring within the source string.</param>
        IScanner<TToken> Create(string input, int extraPosition);

        /// <summary>
        /// Get or set the <see cref="IScannerTable"/> used by the created instances of <see cref="IScanner"/>.
        /// The ScannerTable provides information on specific character representation of language constructs. 
        /// </summary>
        IScannerTable ScannerTable { get; set; }
    }
}
