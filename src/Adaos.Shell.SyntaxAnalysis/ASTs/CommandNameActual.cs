namespace Adaos.Shell.SyntaxAnalysis.ASTs
{
    /// <summary>
    /// A class representing a concrete <see cref="CommandName"/> of the Adaos AST.
    /// </summary>
    public class CommandNameActual : CommandName
    {
        /// <summary>
        /// Get the word with the actual command name value.
        /// </summary>
        public Word Name { get; private set; }

        /// <summary>
        /// Get the position of the first character of the command name.
        /// </summary>
        public override int Position
        {
            get
            {
                return Name.Position;
            }
        }

        /// <summary>
        /// A constructor for the CommanNameActual
        /// </summary>
        /// <param name="name">The actual word node of the AST representing the command name.</param>
        public CommandNameActual(Word name)
        {
            Name = name;
        }

        /// <summary>
        /// A constructor for the CommandNameActual
        /// </summary>
        /// <param name="position">The position of the first character of the command name.</param>
        /// <param name="name">The actual word node of the AST representing the command name.</param>
        public CommandNameActual(int position, Word name) : base(position)
        {
            Name = name;
        }

        /// <summary>
        /// The visit method used to accept an <see cref="IVisitor"/>, 
        /// for an implementation of the visitor pattern.
        /// </summary>
        /// <param name="visitor">The <see cref="IVisitor"/> to "accept".</param>
        /// <param name="obj">An object passed to the visitor.</param>
        public override object Visit(IVisitor visitor, object obj)
        {
            return visitor.Visit(this, obj);
        }

        /// <summary>
        /// Get a string representing the actual command name.
        /// This will be the spelling - or actual source string - associated with this CommandName
        /// AST node.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return Name.Spelling;
        }
    }
}
