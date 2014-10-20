using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adaos.Shell.SyntaxAnalysis.ASTs
{
    abstract public class ProgramSequenceFollow : AST
    {
        public abstract IEnumerable<Command> Commands { get; }

        public ProgramSequenceFollow(int position) : base(position) { }
        public ProgramSequenceFollow() { }
    }
}
