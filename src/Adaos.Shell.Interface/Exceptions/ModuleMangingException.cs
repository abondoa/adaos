using System;
using System.Runtime.Serialization;

namespace Adaos.Shell.Interface.Exceptions
{
    /// <summary>
    /// An exception thrown when an error occurs within the <see cref="IModuleManager"/>.
    /// </summary>
    public class ModuleMangingException : Exception
    {
        /// <summary>
        /// A construcfor for the ModuleMangingException.
        /// </summary>
        public ModuleMangingException() { }

        /// <summary>
        /// A construcfor for the ModuleMangingException.
        /// </summary>
        /// <param name="message">The exception message.</param>
        public ModuleMangingException(string message) 
            : base(message) { }

        /// <summary>
        /// A construcfor for the ModuleMangingException.
        /// </summary>
        /// <param name="message">The exception message.</param>
        /// <param name="innerException">The inner exception.</param>
        public ModuleMangingException(string message, Exception innerException) 
            : base(message, innerException) { }

        /// <summary>
        /// A construcfor for deserializing an ModuleMangingException.
        /// </summary>
        /// <param name="info">The serialization info.</param>
        /// <param name="context">The streaming context with the serialized exception data.</param>
        public ModuleMangingException(SerializationInfo info, StreamingContext context) 
            : base(info, context) { }
    }
}
