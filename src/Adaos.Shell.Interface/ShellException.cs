using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adaos.Shell.Interface
{
    abstract public class ShellException : Exception
    {
        public virtual int Position { get; private set; }

        public ShellException(int position = -1)
            : base()
        {
            Position = position;
        }

        public ShellException(string message, int position = -1)
            : base(message)
        {
            Position = position;
        }

        public ShellException(string message, Exception innerException, int position = -1)
            : base(message, innerException)
        {
            Position = position;
        }

        public ShellException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context,
            int position = -1)
            : base(info, context)
        {
            Position = position;
        }
    }
}
