using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adaos.Shell.Interface;

namespace Adaos.Shell.SyntaxAnalysis.ASTs
{
    abstract public class Environment : AST
    {
        public Environment(int position) : base(position){}
        public Environment() { }
        public abstract string Name { get; }
    }
}
