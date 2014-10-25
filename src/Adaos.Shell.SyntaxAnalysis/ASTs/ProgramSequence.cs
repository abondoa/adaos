using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adaos.Shell.Interface;

namespace Adaos.Shell.SyntaxAnalysis.ASTs
{
    abstract public class ProgramSequence : AST, IProgramSequence
    {
        public abstract IEnumerable<Command> Commands { get; }

        public ProgramSequence(int position) : base(position) { }
        public ProgramSequence() { }

        IEnumerable<ICommand> IProgramSequence<ICommand, ShellException>.Commands
        {
            get { return Commands; }
        }

        public IEnumerable<ShellException> Errors
        {
            get;
            internal set;
        }
    }
}
