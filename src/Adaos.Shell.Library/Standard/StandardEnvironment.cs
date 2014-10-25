using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adaos.Shell.Core;
using Adaos.Shell.Interface;

namespace Adaos.Shell.Library.Standard
{
    public class StandardEnvironment : BaseEnvironment
    {
        public StandardEnvironment(IVirtualMachine vm)
        {
            var envEnv = new EnvironmentEnvironment(vm.Output, vm);
            AddEnvironment(envEnv);
            AddEnvironment(new IOEnvironment(vm.Output, vm.Log));
            AddEnvironment(new CustomEnvironment());
            AddEnvironment(new MathEnvironment(vm.Output));
            AddEnvironment(new ArgumentEnvironment());
            AddEnvironment(new CommandEnvironment(vm.Output, vm));
            AddEnvironment(new ModuleEnvironment(vm.Output, envEnv, vm));
            AddEnvironment(new SyntaxEnvironment(vm));
            AddEnvironment(new ControlStructureEnvironment());
            AddEnvironment(new BareWordsEnvironment());
        }

        public override string Name
        {
            get { return "std"; }
        }
    }
}
