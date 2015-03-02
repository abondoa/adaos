using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adaos.Shell.Interface.Exceptions;

namespace Adaos.Shell.Execution.Exceptions
{
    public class VMException : AdaosException
    {
        public VMException(int position)
            : base(position)
        {
        }

        public VMException(int position,string message)
            : base(message,position)
        {
        }

        public VMException(int position, string message, Exception innerException)
            : base(message, innerException, position)
        {
        }

        public VMException(int position,
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info, context)
        {
        }

        public override string Message
        {
            get
            {
                return "Contextual Error: " + base.Message;
            }
        }
    }
}
