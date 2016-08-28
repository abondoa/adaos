using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adaos.Shell.Interface
{
    public interface IContextBuilder
    {
        IEnumerable<IEnvironmentContext> BuildEnvironments(IVirtualMachine virtualMachine);
    }
}
