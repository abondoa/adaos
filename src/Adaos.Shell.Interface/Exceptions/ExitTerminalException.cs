using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adaos.Shell.Interface.Exceptions
{
    /// <summary>
    /// Thrown to indicate that the terminal should stop running.
    /// </summary>
    public class ExitTerminalException : AdaosException
    {
        public ExitTerminalException() : base() { }

        public ExitTerminalException(string message) : base(message) { }

        public ExitTerminalException(string message, Exception innerException) : base(message, innerException) { }

        public ExitTerminalException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
