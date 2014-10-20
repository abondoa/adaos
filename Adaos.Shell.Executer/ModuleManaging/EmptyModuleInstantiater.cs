using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adaos.Shell.Interface;

namespace Adaos.Shell.Executer.ModuleManaging
{
    class EmptyModuleInstantiater : IModuleInstantiater
    {
        public IModule Instantiate(Type moduleType)
        {
            return (IModule)Activator.CreateInstance(moduleType);
        }
    }
}
