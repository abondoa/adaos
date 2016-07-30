using System;

namespace Adaos.Shell.SyntaxAnalysis.ASTs
{
    /// <summary>
    /// The base class for any Adaos AST node class.
    /// </summary>
    public abstract class AST
    {
        /// <summary>
        /// Get the position of the first character in the source string, 
        /// which is represented by this AST node.
        /// </summary>
        public virtual int Position { get; private set; }


        /// <summary>
        /// A constructor for the AST class.
        /// </summary>
        protected AST()
            : this(-1) { }

        /// <summary>
        /// A constructor for the AST class.
        /// </summary>
        /// <param name="position">The position of the first character in the source string</param>
        protected AST(int position)
        {
            Position = position;
        }

        /// <summary>
        /// The visit method used to accept an <see cref="IVisitor"/>, 
        /// for an implementation of the visitor pattern.
        /// </summary>
        /// <param name="visitor">The <see cref="IVisitor"/> to "accept".</param>
        /// <param name="obj">An object passed to the visitor.</param>
        public abstract object Visit(IVisitor visitor, Object obj);
    }
}
