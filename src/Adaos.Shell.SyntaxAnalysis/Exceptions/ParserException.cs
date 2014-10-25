using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adaos.Shell.Interface;

namespace Adaos.Shell.SyntaxAnalysis.Exceptions
{
    public class ParserException : ShellException
    {
        public ParserException() : base(-1)
        { }

        public ParserException(string message)
            : base(message)
        { }

        public ParserException(string message, Exception innerException)
            : base(message,innerException)
        { }

        public ParserException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info,context)
        { }

        public ParserException(int position)
            : base(position)
        {
        }

        public ParserException(int position,string message)
            : base(message, position)
        {
        }

        public ParserException(int position,string message, Exception innerException)
            : base(message, innerException, position)
        {
        }

        public ParserException(int position,
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context, position)
        {
        }

        public override string Message
        {
            get
            {
                string innerMes = "";
                if (InnerException != null && InnerException.Message != null && InnerException.Message != "")
                {
                    innerMes = " Inner exception message: " + InnerException.Message;
                }
                return "Syntactic Error: " + base.Message + innerMes;
            }
        }
    }
}
