namespace Adaos.Shell.Interface
{
    /// <summary>
    /// An interface describing the scannertable used to define characters for certain
    /// constructs within the Adaos language.
    /// </summary>
    public interface IScannerTable
    {
        /// <summary>
        /// Get or set the character used for defining a pipe.
        /// </summary>
        string Pipe { get; set; }

        /// <summary>
        /// Get or set the character used for defining an executable string.
        /// </summary>
        string Execute { get; set; }

        /// <summary>
        /// Get or set the character used to separate commands.
        /// </summary>
        string CommandSeparator { get; set; }

        /// <summary>
        /// Get or set the character used to separate environments.
        /// </summary>
        string EnvironmentSeparator { get; set; }

        /// <summary>
        /// Get or set the character used to concatenate two commands.
        /// </summary>
        string CommandConcatenator { get; set; }

        /// <summary>
        /// Get or set the escape character.
        /// </summary>
        string Escaper { get; set; }

        /// <summary>
        /// Get or set the character used to separate command arguments.
        /// </summary>
        string ArgumentSeparator { get; set; }

        /// <summary>
        /// Copy the scanner table.
        /// </summary>
        /// <returns>A complete copy of the scanner table.</returns>
        IScannerTable Copy();
    }
}
