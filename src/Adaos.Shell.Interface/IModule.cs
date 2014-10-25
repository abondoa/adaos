using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adaos.Shell.Interface
{
    public interface IModule
    {
        IEnumerable<IEnvironment> Environments { get; }
        string Name { get; }
        IEnumerable<IModuleInstantiater> ModuleInstantiaters { get; }
    }
}
