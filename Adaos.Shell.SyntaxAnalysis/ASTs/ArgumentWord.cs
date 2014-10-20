using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adaos.Shell.SyntaxAnalysis.ASTs
{
    public class ArgumentWord : Argument
    {
        public Word Word { get; private set; }
        public bool Execute { get; private set; }
        public override int Position
        {
            get
            {
                return Word.Position;
            }
        }

        public ArgumentWord(int position, bool execute, Word word)
            : base(position)
        {
            Word = word;
            Execute = execute;
        }

        public ArgumentWord(bool execute, Word word)
        {
            Word = word;
            Execute = execute;
        }

        public override string ToString()
        {
            return Word.Spelling;
        }

        public override object Visit(IVisitor visitor, object obj)
        {
            return visitor.Visit(this, obj);
        }

        public override bool ToExecute
        {
            get { return Execute; }
        }
    }
}
