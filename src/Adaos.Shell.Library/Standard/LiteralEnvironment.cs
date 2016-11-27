using Adaos.Shell.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Adaos.Shell.Interface;
using Adaos.Shell.Interface.Execution;
using Adaos.Common.Extenders;

namespace Adaos.Shell.Library.Standard
{
    public class LiteralEnvironment : BaseVariableEnvironment
    {
        public LiteralEnvironment(IVirtualMachine vm) : base(vm, false)
        {
        }

        public override string Name => "literals";

        public override Command Retrieve(string commandName)
        {
            int temp;
            if (int.TryParse(commandName, out temp))
            {
                return args => HandleVariable(commandName, new[] { new DummyArgument(commandName) }, args.Flatten(), -1);
            }
            return null;
        }
    }
}
