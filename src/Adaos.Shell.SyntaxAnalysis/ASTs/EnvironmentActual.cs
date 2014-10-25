using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adaos.Shell.SyntaxAnalysis.ASTs
{
    public class EnvironmentActual : Environment
    {
        internal Word Word { get; private set; }

        internal EnvironmentActual(Word env)
        {
            Word = env;
        }

        public override object Visit(IVisitor visitor, object obj)
        {
            return visitor.Visit(this, obj);
        }

        public override string Name
        {
            get
            {
                return Word.Spelling;
            }
        }

        public override int Position
        {
            get
            {
                return Word.Position;
            }
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
