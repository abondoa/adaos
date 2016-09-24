using Adaos.Shell.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Adaos.Shell.Interface.Execution;

namespace Adaos.Shell.Execution.Environments
{
    class SystemEnvironmentContextBuilder : IContextBuilder
    {
        public IEnumerable<IEnvironmentContext> BuildEnvironments(IVirtualMachine virtualMachine)
        {
            yield return new SystemEnvironment().AsContext();
        }
    }
}
