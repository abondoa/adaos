using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adaos.Shell.SyntaxAnalysis.ASTs
{
    abstract public class Word : AST, INestedElement
    {
        public abstract string Spelling { get; }
        public Word(int position) : base(position) { }
        public Word() { }

        public override string ToString()
        {
            return Spelling;
        }
    }
}
