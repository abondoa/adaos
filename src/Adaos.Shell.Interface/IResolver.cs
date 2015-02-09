using System;
using System.Collections.Generic;
namespace Adaos.Shell.Interface
{
    public interface IResolver
    {
        IEnvironment GetEnvironmentOf(ICommand command, IEnumerable<IEnvironmentContext> environments);
        Command Resolve(ICommand command, IEnumerable<IEnvironmentContext> environments);
    }
}
