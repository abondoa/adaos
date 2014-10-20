using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adaos.Shell.SyntaxAnalysis.ASTs
{
    public class ArgumentSequenceActual : ArgumentSequence
    {
        public Argument Argument { get; private set; }
        public ArgumentSequence ArgumentSequence { get; private set; }
        public override IEnumerable<Argument> Arguments
        {
            get 
            {
                yield return Argument;
                foreach (Argument arg in ArgumentSequence.Arguments)
                {
                    yield return arg;
                }
            }
        }

        public override int Position
        {
            get
            {
                return Argument.Position;
            }
        }

        public ArgumentSequenceActual(int position, Argument argument, ArgumentSequence argmumentSequence) 
            : base(position)
        {
            Argument = argument;
            ArgumentSequence = argmumentSequence;
        }

        public ArgumentSequenceActual(Argument argument, ArgumentSequence argmumentSequence)
        {
            Argument = argument;
            ArgumentSequence = argmumentSequence;
        }

        public override object Visit(IVisitor visitor, object obj)
        {
            return visitor.Visit(this, obj);
        }
    }
}
