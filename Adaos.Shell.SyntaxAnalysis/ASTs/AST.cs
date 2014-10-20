using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adaos.Shell.SyntaxAnalysis.ASTs
{
    abstract public class AST
    {
        public virtual int Position { get; private set; }

        public AST(int position)
        {
            Position = position;
        }

        public AST()
        {
            Position = -1;
        }

        public abstract Object Visit(IVisitor visitor, Object obj);
    }
}
