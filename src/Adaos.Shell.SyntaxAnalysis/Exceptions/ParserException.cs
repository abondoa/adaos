using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adaos.Shell.Interface.Exceptions;

namespace Adaos.Shell.SyntaxAnalysis.Exceptions
{
    public class ParserException : SyntacticException
    {
        public ParserException() : base(-1)
        { }

        public ParserException(string message)
            : base(-1,message)
        { }

        public ParserException(string message, Exception innerException)
            : base(-1,message,innerException)
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
            : base(position,message)
        {
        }

        public ParserException(int position,string message, Exception innerException)
            : base(position,message, innerException)
        {
        }

        public ParserException(int position,
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
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
