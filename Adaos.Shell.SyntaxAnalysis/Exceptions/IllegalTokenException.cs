using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adaos.Shell.SyntaxAnalysis.Exceptions
{
    class IllegalTokenException : ParserException
    {
        public IEnumerable<Tokens.TokenKind> Expected { get; private set; }
        public Tokens.Token Received { get; private set; }

        public IllegalTokenException(Tokens.Token received, params Tokens.TokenKind[] expected)
            : base()
        {
            Received = received;
            Expected = expected;
        }

        public IllegalTokenException(Tokens.Token received, string message, params Tokens.TokenKind[] expected)
            : base(message)
        {
            Received = received;
            Expected = expected;
        }

        public IllegalTokenException(
            Tokens.Token received, string message,
            Exception innerException,
            params Tokens.TokenKind[] expected)
            : base(message,innerException)
        {
            Received = received;
            Expected = expected;
        }

        public IllegalTokenException(
            Tokens.Token received,
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context,
            params Tokens.TokenKind[] expected)
            : base(info,context)
        {
            Received = received;
            Expected = expected;
        }

        public override int Position
        {
            get
            {
                return Received.Position;
            }
        }

        public override string Message
        {
            get
            {
                string result = base.Message + " Expected Token of kind: ";
                result += Expected.First().ToString();
                foreach (var exp in Expected.Skip(1))
                { 
                    result += " or " + exp.ToString();
                }

                string receivedString = "'"+Received.ToString()+"'";
                if (Received.Kind == Tokens.TokenKind.EOF)
                {
                    receivedString = "EOF token";
                }
                result += ". But received: " + receivedString;

                return result;
            }
        }
    }
}
