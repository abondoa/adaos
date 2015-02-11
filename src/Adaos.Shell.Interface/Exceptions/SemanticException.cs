using System;
using System.Runtime.Serialization;

namespace Adaos.Shell.Interface.Exceptions
{
    /// <summary>
    /// An exception thrown when a semantic error occurs during parsing of a Adaos language source string.
    /// </summary>
    public class SemanticException : AdaosException
    {
        /// <summary>
        /// A constructor for the SemanticException.
        /// </summary>
        /// <param name="position">The position within the source string of the semantic error.</param>
        public SemanticException(int position)
            : base(position) { }

        /// <summary>
        /// A constructor for the SemanticException.
        /// </summary>
        /// <param name="position">The position within the source string of the semantic error.</param>
        /// <param name="message">The exception message describing the error.</param>
        public SemanticException(int position, string message)
            : base(message, position) { }

        /// <summary>
        /// A constructor for the SemanticException.
        /// </summary>
        /// <param name="position">The position within the source string of the semantic error.</param>
        /// <param name="message">The exception message describing the error.</param>
        /// <param name="innerException">The inner exception.</param>
        public SemanticException(int position, string message, Exception innerException)
            : base(message, innerException, position) { }

        /// <summary>
        /// A constructor for deserializing a SemanticException.
        /// </summary>
        /// <param name="position">The position within the source string of the semantic error.</param>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context with the serialized exception data.</param>
        public SemanticException(int position, SerializationInfo info, StreamingContext context)
            : base(info, context, position) { }

        /// <summary>
        /// Get the error message of the semantic exception.
        /// </summary>
        public override string Message
        {
            get
            {
                return "Semantic Error: " + base.Message;
            }
        }
    }
}
