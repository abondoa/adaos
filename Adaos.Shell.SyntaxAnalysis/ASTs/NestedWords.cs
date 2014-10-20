using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adaos.Shell.SyntaxAnalysis.ASTs
{
    /// <summary>
    /// This AST class covers both inner double and inner single
    /// </summary>
    abstract public class NestedWords : AST
    {
        public NestedWords(int position) : base(position) { }
        public NestedWords() { }
    }
}
