using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adaos.Shell.Interface;

namespace Adaos.Shell.Core
{
    public class DummyProgramSequence: IProgramSequence
    {
        public IEnumerable<ICommand> Commands
        {
            get;
            private set;
        }

        public IEnumerable<ShellException> Errors
        {
            get;
            private set;
        }

        public DummyProgramSequence(params ICommand[] commands) : this(new ShellException[0],commands) {  }

        public DummyProgramSequence(IEnumerable<ShellException> errors, params ICommand[] commands)
        {
            Errors = errors;
            Commands = commands;
        }
    }
}
