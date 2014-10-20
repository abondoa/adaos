using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adaos.Shell.SyntaxAnalysis.ASTs
{
    public class ProgramSequenceFollowActual : ProgramSequenceFollow
    {
        public Command Command { get; private set; }
        public ProgramSequenceFollow FollowingCommands { get; private set; }
        public override IEnumerable<Command> Commands 
        {
            get 
            {
                yield return Command;
                foreach (Command com in FollowingCommands.Commands)
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

        public ProgramSequenceFollowActual(int position, Command command, ProgramSequenceFollow followingCommands)
            : base(position)
        {
            Command = command;
            FollowingCommands = followingCommands;
        }

        public ProgramSequenceFollowActual(Command command, ProgramSequenceFollow followingCommands)
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
