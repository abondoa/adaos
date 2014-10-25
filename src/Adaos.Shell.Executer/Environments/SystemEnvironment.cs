using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adaos.Shell.Interface;
using System.IO;
using Adaos.Shell.SyntaxAnalysis.Exceptions;
using Adaos.Shell.Core;
using Adaos.Shell.Interface.Exceptions;

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
        }

        private IEnumerable<IArgument> Exit(IEnumerable<IArgument> args)
        {
            throw new ExitTerminalException("Bye!");
        }
    }
}
