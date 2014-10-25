using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adaos.Shell.SyntaxAnalysis.ASTs
{
    public class ArgumentWord : Argument
    {
        public Word Word { get; private set; }
        public override int Position
        {
            get
            {
                return Word.Position;
            }
        }

        public ArgumentWord(int position, bool execute, Word word, Word wordName = null)
            : base(position,execute,wordName)
        {
            Word = word;
        }

        public ArgumentWord(bool execute, Word word, Word wordName = null)
            : base(execute, wordName)
        {
            Word = word;
        }

        public override object Visit(IVisitor visitor, object obj)
        {
            return visitor.Visit(this, obj);
        }

        public override string Value
        {
            get { return Word.Spelling; }
        }
    }
}
