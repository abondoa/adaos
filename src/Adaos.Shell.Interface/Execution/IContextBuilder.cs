using System.Collections.Generic;

namespace Adaos.Shell.Interface.Execution
{
    public interface IContextBuilder
    {
        IEnumerable<IEnvironmentContext> BuildEnvironments(IVirtualMachine virtualMachine);
    }
}
