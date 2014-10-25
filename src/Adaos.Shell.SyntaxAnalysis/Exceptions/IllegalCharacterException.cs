using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adaos.Shell.SyntaxAnalysis.Exceptions
{
    class IllegalCharacterException : ScannerException
    {
        public char Expected { get; private set; }
        public char Received { get; private set; }

        public IllegalCharacterException(char expected, char received) : base()
        {
            Expected = expected;
            Received = received;
        }

        public IllegalCharacterException(char expected, char received, string message)
            : base(message)
        {
            Expected = expected;
            Received = received;
        }

        public IllegalCharacterException(char expected, char received, string message, Exception innerException)
            : base(message,innerException)
        {
            Expected = expected;
            Received = received;
        }

        public IllegalCharacterException(char expected, char received,
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context)
            : base(info,context)
        {
            Expected = expected;
            Received = received;
        }

        public override string Message
        {
            get
            {
                string result = base.Message + " Expected Token of kind: ";
                result += Expected;
                result += ". But received: " + Received;

                return result;
            }
        }
    }
}
