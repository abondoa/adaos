using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adaos.Shell.SyntaxAnalysis.ASTs
{
    public class ArgumentSequenceEmpty : ArgumentSequence
    {
        public override IEnumerable<Argument> Arguments
        {
            get 
            {
                yield break;
            }
        }

        public ArgumentSequenceEmpty(int position) : base(position) 
        {
        }

        public override object Visit(IVisitor visitor, object obj)
        {
            return visitor.Visit(this, obj);
        }
    }
}
