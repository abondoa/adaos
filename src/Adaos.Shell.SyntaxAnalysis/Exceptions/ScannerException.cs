using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adaos.Shell.Interface;

namespace Adaos.Shell.SyntaxAnalysis.Exceptions
{
    public class ScannerException : ShellException
    {
        public ScannerException(int position) : base(position)
        {
        }

        public ScannerException(int position, string message)
            : base(message,position)
        {
        }

        public ScannerException(int position, string message, Exception innerException)
            : base(message, innerException, position)
        {
        }

        public ScannerException(int position,
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context, position)
        {
        }

        public ScannerException()
            : base(-1)
        {
        }

        public ScannerException(string message)
            : base(message, -1)
        {
        }

        public ScannerException(string message, Exception innerException)
            : base(message, innerException, -1)
        {
        }

        public ScannerException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context, -1)
        {
        }

        public override string Message
        {
            get
            {
                return "Syntactic Error: " + base.Message;
            }
        }
    }
}
