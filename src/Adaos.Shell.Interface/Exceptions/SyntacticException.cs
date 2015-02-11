using System;
using System.Runtime.Serialization;

namespace Adaos.Shell.Interface.Exceptions
{
    /// <summary>
    /// An exception thrown when an error occurs during the syntactic analysis of a command.
    /// Should only be thrown in SyntacticAnalysis project.
    /// </summary>
    public class SyntacticException : AdaosException
    {
        /// <summary>
        /// A constructor for the SyntacticException.
        /// </summary>
        /// <param name="position">The position within the source string of the syntax error.</param>
        public SyntacticException(int position)
            : base(position) { }

        /// <summary>
        /// A constructor for the SyntacticException.
        /// </summary>
        /// <param name="position">The position within the source string of the syntax error.</param>
        /// <param name="message">The exception message describing the error.</param>
        /// <param name="innerException">The inner exception.</param>
        public SyntacticException(int position, string message)
            : base(message,position) { }

        /// <summary>
        /// A constructor for the SyntacticException.
        /// </summary>
        /// <param name="position">The position within the source string of the syntax error.</param>
        /// <param name="message">The exception message describing the error.</param>
        /// <param name="innerException">The inner exception.</param>
        public SyntacticException(int position, string message, Exception innerException)
            : base(message, innerException, position) { }

        /// <summary>
        /// A constructor for deserializing a SyntacticException.
        /// </summary>
        /// <param name="position">The position within the source string of the syntax error.</param>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context with the serialized exception data.</param>
        public SyntacticException(int position, SerializationInfo info, StreamingContext context)
            : base(info, context, position) { }

        /// <summary>
        /// Get the error message of the syntactic exception.
        /// </summary>
        public override string Message
        {
            get
            {
                return "Syntactic Error: " + base.Message;
            }
        }
    }
}
