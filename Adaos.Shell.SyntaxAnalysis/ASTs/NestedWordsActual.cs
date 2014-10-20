using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adaos.Shell.SyntaxAnalysis.ASTs
{
    public class NestedWordsActual : NestedWords
    {
        internal Tokens.Token Nest { get; private set; }

        internal NestedWordsActual(Tokens.Token nest)
        {
            if (nest.Kind != Tokens.TokenKind.NESTEDWORDS)
            {
                throw new ArgumentException("nest must be of KIND NESTEDWORDS");
            }

            Nest = nest;
        }

        public override string ToString()
        {
            return Nest.Spelling.Substring(1, Nest.Spelling.Length - 2);
        }

        public override int Position
        {
            get
            {
                return Nest.Position + (Nest.Spelling.Length - (ToString().TrimStart().Length + 1));
            }
        }

        public override object Visit(IVisitor visitor, object obj)
        {
            return visitor.Visit(this, obj);
        }
    }
}
