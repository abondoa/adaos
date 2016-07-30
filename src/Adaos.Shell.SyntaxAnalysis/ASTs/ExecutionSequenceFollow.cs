using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adaos.Shell.SyntaxAnalysis.ASTs
{
    abstract public class ExecutionSequenceFollow : AST
    {
        public abstract IEnumerable<Execution> Commands { get; }

        public ExecutionSequenceFollow(int position) : base(position) { }
        public ExecutionSequenceFollow() { }
    }
}
