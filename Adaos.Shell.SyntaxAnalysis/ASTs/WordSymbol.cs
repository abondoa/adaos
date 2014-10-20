using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adaos.Shell.SyntaxAnalysis.ASTs
{
    public class WordSymbol : Word
    {
        internal Tokens.Token WordToken { get; private set; }
        public override string Spelling
        {
            get 
            {
                return WordToken.Spelling;
            }
        }

        public override int Position
        {
            get
            {
                return WordToken.Position;
            }
        }

        internal WordSymbol(Tokens.Token wordToken)
            : base(wordToken.Position)
        {
            if (wordToken.Kind != Tokens.TokenKind.MATH_SYMBOL)
            {
                throw new ArgumentException("The token inserted into the WORD AST must be of the kind WORD");
            }
            WordToken = wordToken;
        }

        public override object Visit(IVisitor visitor, object obj)
        {
            return visitor.Visit(this, obj);
        }
    }
}
