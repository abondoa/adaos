using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adaos.Shell.Interface.SyntaxAnalysis;
using Adaos.Shell.Interface.Exceptions;

namespace Adaos.Shell.Core
{
    public class DummyExecutionSequence: IExecutionSequence
    {
        public IEnumerable<IExecution> Executions
        {
            get;
            private set;
        }

        public IEnumerable<AdaosException> Errors
        {
            get;
            private set;
        }

        public DummyExecutionSequence(params IExecution[] commands) : this(new AdaosException[0],commands) {  }

        public DummyExecutionSequence(IEnumerable<AdaosException> errors, params IExecution[] commands)
        {
            Errors = errors;
            Executions = commands;
        }
    }
}
