using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adaos.Shell.Interface;
using System.IO;
using Adaos.Shell.SyntaxAnalysis.Exceptions;
using Adaos.Shell.Executer.Extenders;
using Adaos.Shell.Core;

namespace Adaos.Shell.Executer.Environments
{
    class SystemEnvironment : Environment
    {
        virtual protected int _test {get;set;}
        virtual protected IVirtualMachine _vm { get; private set; }
        private readonly List<string[]> CommonFlagsWithAlias = new List<string[]>
        { 
            new string[]{"-silent","-si"},
            new string[]{"-verbose","-v"}
        };

        public override string Name
        {
            get { return "system"; } 
        }

        public SystemEnvironment(IVirtualMachine vm = null)
        {
            _vm = vm;
            Bind(Exit,"exit","quit");
        }

        private IEnumerable<IArgument> Exit(IEnumerable<IArgument> args)
        {
            throw new ExitShellException("Bye!");
        }
    }
}
