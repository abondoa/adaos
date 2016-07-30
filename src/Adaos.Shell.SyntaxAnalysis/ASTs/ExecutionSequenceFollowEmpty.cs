using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adaos.Shell.SyntaxAnalysis.ASTs
{
    public class ExecutionSequenceFollowEmpty : ExecutionSequenceFollow
    {
        public override IEnumerable<Execution> Commands
        {
            get
            {
                return new List<Execution>();
            }
        }

        public ExecutionSequenceFollowEmpty(int position) : base(position) { }

        public override object Visit(IVisitor visitor, object obj)
        {
            return visitor.Visit(this, obj);
        }
    }

    
}
