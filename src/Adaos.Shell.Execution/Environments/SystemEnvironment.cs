using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Adaos.Shell.SyntaxAnalysis.Exceptions;
using Adaos.Shell.Core;
using Adaos.Shell.Interface.Exceptions;
using Adaos.Shell.Interface.SyntaxAnalysis;

namespace Adaos.Shell.Execution.Environments
{
    class SystemEnvironment : BaseEnvironment
    {
        public override string Name
        {
            get { return "system"; } 
        }

        public SystemEnvironment()
        {
            Bind(Exit,"exit","quit");
            Bind(Adaos, "adaos");
        }

        private IEnumerable<IArgument> Exit(IEnumerable<IArgument> args)
        {
            throw new ExitTerminalException("Bye!");
        }

        private IEnumerable<IArgument> Adaos(IEnumerable<IArgument> args)
        {
            yield return new DummyArgument("Adaos Debugging And Operating Shell");
        }
    }
}
