using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adaos.Shell.Interface;
using Adaos.Shell.Interface.Exceptions;

namespace Adaos.Shell.Core
{
    public class DummyProgramSequence: IProgramSequence
    {
        public IEnumerable<ICommand> Commands
        {
            get;
            private set;
        }

        public IEnumerable<AdaosException> Errors
        {
            get;
            private set;
        }

        public DummyProgramSequence(params ICommand[] commands) : this(new AdaosException[0],commands) {  }

        public DummyProgramSequence(IEnumerable<AdaosException> errors, params ICommand[] commands)
        {
            Errors = errors;
            Commands = commands;
        }
    }
}
