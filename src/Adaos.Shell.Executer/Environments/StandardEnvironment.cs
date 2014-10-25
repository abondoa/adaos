using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adaos.Shell.Interface;

namespace Adaos.Shell.Executer.Environments
{
    public class StandardEnvironment : Environment
    {
        public StandardEnvironment(IVirtualMachine vm)
        {
            var envEnv = new Environments.EnvironmentEnvironment(vm.Output, vm);
            AddEnvironment(envEnv);
            AddEnvironment(new Environments.IOEnvironment(vm.Output, vm.Log));
            AddEnvironment(new Environments.CustomEnvironment());
            AddEnvironment(new Environments.MathEnvironment(vm.Output));
            AddEnvironment(new Environments.ArgumentEnvironment());
            AddEnvironment(new Environments.CommandEnvironment(vm.Output, vm));
            AddEnvironment(new Environments.ModuleEnvironment(vm.Output, envEnv, vm));
            AddEnvironment(new Environments.SyntaxEnvironment(vm));
            AddEnvironment(new Environments.ControlStructureEnvironment());
            AddEnvironment(new Environments.BareWordsEnvironment());
        }

        public override string Name
        {
            get { return "std"; }
        }
    }
}
