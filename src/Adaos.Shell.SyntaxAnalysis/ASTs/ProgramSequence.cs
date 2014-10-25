using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adaos.Shell.Interface;
using Adaos.Shell.Interface.Exceptions;

namespace Adaos.Shell.SyntaxAnalysis.ASTs
{
    abstract public class ProgramSequence : AST, IProgramSequence
    {
        public abstract IEnumerable<Command> Commands { get; }

        public ProgramSequence(int position) : base(position) { }
        public ProgramSequence() { }

        IEnumerable<ICommand> IProgramSequence<ICommand, AdaosException>.Commands
        {
            get { return Commands; }
        }

        public IEnumerable<AdaosException> Errors
        {
            get;
            internal set;
        }
    }
}
