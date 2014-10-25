using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adaos.Shell.Interface.Exceptions
{
    /// <summary>
    /// All exceptions which are to be shown to the user of the shell should be
    /// of this type.
    /// </summary>
    abstract public class AdaosException : Exception
    {
        /// <summary>
        /// The position where the error occured in the string being executed.
        /// -1 for unknown/not applicable position.
        /// </summary>
        public virtual int Position { get; private set; }

        public AdaosException(int position = -1)
            : base()
        {
            Position = position;
        }

        public AdaosException(string message, int position = -1)
            : base(message)
        {
            Position = position;
        }

        public AdaosException(string message, Exception innerException, int position = -1)
            : base(message, innerException)
        {
            Position = position;
        }

        public AdaosException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context,
            int position = -1)
            : base(info, context)
        {
            Position = position;
        }
    }
}
