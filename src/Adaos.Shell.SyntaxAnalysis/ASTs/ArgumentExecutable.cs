using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adaos.Shell.SyntaxAnalysis.ASTs
{
    public class ArgumentExecutable : Argument
    {
        ExecutionSequence ExecutionSequence { get; }

        public ArgumentExecutable(ExecutionSequence executionSequence, int position, bool execute, Word wordName = null) : base(position, execute, wordName)
        {
            ExecutionSequence = executionSequence;
        }

        public override string Value => ExecutionSequence.ToString();

        public override object Visit(IVisitor visitor, object obj)
        {
            return visitor.Visit(this, obj);
        }
    }
}
