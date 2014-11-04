using System;
using System.Collections.Generic;

namespace Adaos.Shell.Interface
{
    public interface IEnvironmentContext : IEnvironment
    {
        IEnumerable<string> EnvironmentNames { get; }
    }
}
