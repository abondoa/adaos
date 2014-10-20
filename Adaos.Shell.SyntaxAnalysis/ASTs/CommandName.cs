using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adaos.Shell.SyntaxAnalysis.ASTs
{
    abstract public class CommandName : AST
    {
        public CommandName(int position) : base(position) { }
        public CommandName() { }
        
    }
}
