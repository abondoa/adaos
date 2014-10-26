using System;
using System.Collections.Generic;

namespace Adaos.Shell.Interface
{
    public interface IEnvironmentContext : IEnvironment
    {
        string QualifiedName(string separator);
        IEnumerable<string> EnvironmentNames { get; }
    }
}
