using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adaos.Shell.Interface;

namespace Adaos.Shell.Executer.ModuleManaging
{
    class VirtualMachineModuleInstantiater : IModuleInstantiater
    {
        private IVirtualMachine _vm;
        public VirtualMachineModuleInstantiater(IVirtualMachine vm)
        {
            _vm = vm;
        }

        public IModule Instantiate(Type moduleType)
        {
            return (IModule)Activator.CreateInstance(moduleType,new object[]{_vm});
        }
    }
}
