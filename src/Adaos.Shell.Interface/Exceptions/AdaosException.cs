using System;
using System.Runtime.Serialization;

namespace Adaos.Shell.Interface.Exceptions
{
    /// <summary>
    /// An abstract exception class used for all the execeptions 
    /// which are to be shown to the user of the shell.
    /// </summary>
    public abstract class AdaosException : Exception
    {
        /// <summary>
        /// Get the position where the error occured in the string being executed.
        /// -1 for unknown/not applicable position.
        /// </summary>
        public virtual int Position { get; private set; }

        /// <summary>
        /// A construcfor for the AdaosException.
        /// </summary>
        /// <param name="position">The position in which the error occured. Default is -1.</param>
        protected AdaosException(int position = -1)
            : base()
        {
            Position = position;
        }

        /// <summary>
        /// A construcfor for the AdaosException.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="position">The position in which the error occured. Default is -1.</param>
        protected AdaosException(string message, int position = -1)
            : base(message)
        {
            Position = position;
        }

        /// <summary>
        /// A construcfor for the AdaosException.
        /// </summary>
        /// <param name="message">The error message.</param>
        /// <param name="innerException">The inner exception.</param>
        /// <param name="position">The position in which the error occured. Default is -1.</param>
        protected AdaosException(string message, Exception innerException, int position = -1)
            : base(message, innerException)
        {
            Position = position;
        }

        /// <summary>
        /// A construcfor for deserializing an AdaosException.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context with the serialized exception data.</param>
        /// <param name="position">The position in which the error occured. Default is -1.</param>
        protected AdaosException(SerializationInfo info, StreamingContext context, int position = -1)
            : base(info, context)
        {
            Position = position;
        }
    }
}
