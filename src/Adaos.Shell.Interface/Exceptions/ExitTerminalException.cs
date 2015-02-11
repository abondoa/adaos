using System;
using System.Runtime.Serialization;

namespace Adaos.Shell.Interface.Exceptions
{
    /// <summary>
    /// An exception thrown to indicate that the terminal should stop running.
    /// </summary>
    public class ExitTerminalException : AdaosException
    {
        /// <summary>
        /// A construcfor for the ExitTerminalException.
        /// </summary>
        public ExitTerminalException() { }

        /// <summary>
        /// A construcfor for the ExitTerminalException.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public ExitTerminalException(string message) 
            : base(message) { }

        /// <summary>
        /// A construcfor for the ExitTerminalException.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public ExitTerminalException(string message, Exception innerException) 
            : base(message, innerException) { }

        /// <summary>
        /// A construcfor for deserializing an ExitTerminalException.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context with the serialized exception data.</param>
        public ExitTerminalException(SerializationInfo info, StreamingContext context) 
            : base(info, context) { }
    }
}
