using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adaos.Shell.SyntaxAnalysis.ASTs
{
    public class ProgramSequenceFollowEmpty : ProgramSequenceFollow
    {
        public override IEnumerable<Command> Commands
        {
            get
            {
                return new List<Command>();
            }
        }

        public ProgramSequenceFollowEmpty(int position) : base(position) { }

        public override object Visit(IVisitor visitor, object obj)
        {
            return visitor.Visit(this, obj);
        }
    }

    
}
