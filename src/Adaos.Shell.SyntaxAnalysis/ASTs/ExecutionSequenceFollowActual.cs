using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adaos.Shell.SyntaxAnalysis.ASTs
{
    public class ExecutionSequenceFollowActual : ExecutionSequenceFollow
    {
        public Execution Execution { get; private set; }
        public ExecutionSequenceFollow FollowingExecutions { get; private set; }
        public override IEnumerable<Execution> Commands 
        {
            get 
            {
                yield return Execution;
                foreach (Execution com in FollowingExecutions.Commands)
                {
                    yield return com;
                }
            }
        }
        public override int Position
        {
            get
            {
                return Execution.Position;
            }
        }

        public ExecutionSequenceFollowActual(int position, Execution command, ExecutionSequenceFollow followingCommands)
            : base(position)
        {
            Execution = command;
            FollowingExecutions = followingCommands;
        }

        public ExecutionSequenceFollowActual(Execution command, ExecutionSequenceFollow followingCommands)
        {
            Execution = command;
            FollowingExecutions = followingCommands;
        }

        public override object Visit(IVisitor visitor, object obj)
        {
            return visitor.Visit(this, obj);
        }
    }
}
