using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adaos.Shell.Interface;

namespace Adaos.Shell.SyntaxAnalysis.Tokens
{
    public class Token
    {
        public int Position { get; private set; }
        public string Spelling { get; private set; }
        public TokenKind Kind { get; private set; }

        public Token(int position, string spelling, TokenKind kind)
        {
            Position = position;
            Spelling = spelling;
            Kind = kind;
        }

        public override string ToString()
        {
            return Spelling;
        }
    }
}
