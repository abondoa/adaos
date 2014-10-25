using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adaos.Shell.Interface;

namespace Adaos.Shell.SyntaxAnalysis.ASTs
{
    public class CommandActual : CommandBase
    {
        public CommandActual(int position, CommandName commandName, ArgumentSequence arguments) : base(position,commandName,arguments)
        {
        }

        public CommandActual(CommandName commandName, ArgumentSequence arguments) : base(commandName,arguments)
        {
        }

        public override object Visit(IVisitor visitor, object obj)
        {
            return visitor.Visit(this, obj);
        }

        public override IEnumerable<string> EnvironmentNames
        {
            get { yield break; }
        }
    }
}
