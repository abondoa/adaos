using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adaos.Shell.Interface;

namespace Adaos.Shell.SyntaxAnalysis.ASTs
{
    abstract public class CommandBase : Command
    {
        public CommandName CommName { get; private set; }
        public ArgumentSequence Args { get; private  set; }
        public override int Position
        {
            get
            {
                return CommName.Position;
            }
        }
        public override string CommandName
        {
            get
            {
                return CommName.ToString();
            }
        }

        public override IEnumerable<IArgument> Arguments
        {
            get
            {
                foreach (Argument arg in Args.Arguments)
                {
                    yield return arg;
                }
            }
        }

        public CommandBase(int position, CommandName commandName, ArgumentSequence arguments) : base(position)
        {
            CommName = commandName;
            Args = arguments;
        }

        public CommandBase(CommandName commandName, ArgumentSequence arguments)
        {
            CommName = commandName;
            Args = arguments;
        }
    }
}
