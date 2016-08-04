namespace Adaos.Shell.Interface
{
    /// <summary>
    /// An interface describing a parser for the Adaos shell language.
    /// </summary>
    /// <typeparam name="ProgramType">The type of the Adaos program to parse. 
    /// This is the root of the parse tree/concrete syntax tree to be created when parsing the Adaos language 
    /// and thus is required to implement the <see cref="IExecutionSequence"/> interface.</typeparam>
    public interface IShellParser<ProgramType> where ProgramType : IExecutionSequence
    {
        /// <summary>
        /// Parse an input string in the Adaos language. 
        /// This will turn the source input string into a parse tree/concrete syntax tree, 
        /// representing the syntactic structure of the input.
        /// </summary>
        /// <param name="input">The input string to parse.</param>
        /// <returns>A parse tree.</returns>
        ProgramType Parse(string input);

        /// <summary>
        /// Parse an input string in the Adaos language. 
        /// This will turn the source input string into a parse tree/concrete syntax tree, 
        /// representing the syntactic structure of the input.
        /// </summary>
        /// <param name="input">The input string to parse.</param>
        /// <param name="initialPosition">The initial position of the input string.
        /// This is used if the parsed string is a substring of some larger source.</param>
        /// <returns>A parse tree.</returns>
        ProgramType Parse(string input,int initialPosition);

        /// <summary>
        /// Get or set the scanner table used during the scanning part of the parsing. 
        /// The ScannerTable provides information on specific character representation of language constructs. 
        /// </summary>
        IScannerTable ScannerTable { get; set; }
    }

    /// <summary>
    /// An interface describing a parser for the Adaos shell language.
    /// </summary>
    public interface IShellParser : IShellParser<IExecutionSequence> 
    {
    }
}
