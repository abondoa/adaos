using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adaos.Shell.Interface;
using Adaos.Shell.Interface.Exceptions;

namespace Adaos.Shell.SyntaxAnalysis.Exceptions
{
    public class ScannerException : SyntacticException
    {
        public ScannerException(int position) : base(position)
        {
        }

        public ScannerException(int position, string message)
            : base(position,message)
        {
        }

        public ScannerException(int position, string message, Exception innerException)
            : base(position,message, innerException)
        {
        }

        public ScannerException(System.Runtime.Serialization.SerializationInfo info,
                                System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }

        public ScannerException()
            : base(-1)
        {
        }

        public ScannerException(string message)
            : base(-1,message)
        {
        }

        public ScannerException(string message, Exception innerException)
            : base(-1, message, innerException)
        {
        }
    }
}
