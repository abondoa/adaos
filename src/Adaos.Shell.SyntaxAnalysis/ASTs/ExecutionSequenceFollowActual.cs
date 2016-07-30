using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adaos.Shell.SyntaxAnalysis.ASTs
{
    public class ExecutionSequenceFollowActual : ExecutionSequenceFollow
    {
        public Execution Command { get; private set; }
        public ExecutionSequenceFollow FollowingCommands { get; private set; }
        public override IEnumerable<Execution> Commands 
        {
            get 
            {
                yield return Command;
                foreach (Execution com in FollowingCommands.Commands)
                {
                    yield return com;
                }
            }
        }
        public override int Position
        {
            get
            {
                return Command.Position;
            }
        }

        public ExecutionSequenceFollowActual(int position, Execution command, ExecutionSequenceFollow followingCommands)
            : base(position)
        {
            Command = command;
            FollowingCommands = followingCommands;
        }

        public ExecutionSequenceFollowActual(Execution command, ExecutionSequenceFollow followingCommands)
        {
            Command = command;
            FollowingCommands = followingCommands;
        }

        public override object Visit(IVisitor visitor, object obj)
        {
            return visitor.Visit(this, obj);
        }
    }
}
