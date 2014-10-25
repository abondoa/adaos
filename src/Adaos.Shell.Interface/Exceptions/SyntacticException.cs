using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adaos.Shell.Interface.Exceptions
{
    /// <summary>
    /// Exceptions occuring during the syntactic analysis of a program-sequence.
    /// Should only be thrown in SyntacticAnalysis project.
    /// </summary>
    public class SyntacticException : AdaosException
    {
        public SyntacticException(int position)
            : base(position)
        {
        }

        public SyntacticException(int position,string message)
            : base(message,position)
        {
        }

        public SyntacticException(int position, string message, Exception innerException)
            : base(message, innerException, position)
        {
        }

        public SyntacticException(int position,
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context, position)
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
