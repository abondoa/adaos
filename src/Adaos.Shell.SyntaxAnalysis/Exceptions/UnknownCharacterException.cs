using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adaos.Shell.SyntaxAnalysis.Exceptions
{
    class UnknownCharacterException : ScannerException
    {
        public char Received { get; private set; }

        public UnknownCharacterException(char received) : base()
        {
            Received = received;
        }

        public UnknownCharacterException(char received, string message)
            : base(message)
        {
            Received = received;
        }

        public UnknownCharacterException(char received, string message, Exception innerException)
            : base(message,innerException)
        {
            Received = received;
        }

        public UnknownCharacterException(char received,
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info,context)
        {
            Received = received;
        }

        public override string Message
        {
            get
            {
                string result = base.Message + " Unknown character received: " + Received;
                return result;
            }
        }
    }
}
