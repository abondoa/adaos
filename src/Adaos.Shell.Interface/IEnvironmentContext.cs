using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adaos.Shell.Interface
{
    public interface IEnvironmentContext : IEnvironment
    {
        string QualifiedName { get; }
    }
}
