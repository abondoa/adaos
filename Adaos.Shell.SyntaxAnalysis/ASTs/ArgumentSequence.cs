using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adaos.Shell.SyntaxAnalysis.ASTs
{
    abstract public class ArgumentSequence : AST
    {
        public abstract IEnumerable<Argument> Arguments {get;}
        public ArgumentSequence() { }

        public ArgumentSequence(int position) : base(position) { }
    }
}
