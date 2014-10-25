using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adaos.Shell.Interface
{
    public class ExitShellException : ShellException
    {
        public ExitShellException() : base() { }

        public ExitShellException(string message) : base(message) { }

        public ExitShellException(string message, Exception innerException) : base(message, innerException) { }

        public ExitShellException(System.Runtime.Serialization.SerializationInfo info, System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}
