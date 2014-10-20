using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adaos.Shell.Interface
{
    public class SemanticException : ShellException
    {
        public SemanticException(int position)
            : base(position)
        {
        }

        public SemanticException(int position,string message)
            : base(message,position)
        {
        }

        public SemanticException(int position, string message, Exception innerException)
            : base(message, innerException, position)
        {
        }

        public SemanticException(int position,
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context, position)
        {
        }

        public override string Message
        {
            get
            {
                return "Semantic Error: " + base.Message;
            }
        }
    }
}
