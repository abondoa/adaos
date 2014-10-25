using System;
namespace Adaos.Shell.Interface
{
    public interface IResolver
    {
        Adaos.Shell.Interface.IEnvironment GetEnvironmentOf(Adaos.Shell.Interface.ICommand command, System.Collections.Generic.IEnumerable<Adaos.Shell.Interface.IEnvironment> environments);
        Adaos.Shell.Interface.Command Resolve(Adaos.Shell.Interface.ICommand command, System.Collections.Generic.IEnumerable<Adaos.Shell.Interface.IEnvironment> environments);
    }
}
