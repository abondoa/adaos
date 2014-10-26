using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adaos.Shell.Interface
{
    public interface IEnvironment
    {
        string Name { get; }
        IEnvironmentUniqueIdentifier Identifier {get;}
        void Bind(Command command, params string[] commandNames);
        Command Retrieve(string commandName); //Should return null if not existing
        IEnumerable<string> Commands { get; }
        void UnBind(string commandName);
        bool AllowUnbinding { get; }
        IEnumerable<IEnvironmentUniqueIdentifier> Dependencies { get; }
        IEnumerable<IEnvironment> ChildEnvironments { get; }
        IEnvironment ChildEnvironment(string childEnvironmentName);
        void AddEnvironment(IEnvironment environment);
        void RemoveEnvironment(IEnvironment environment);
        IEnvironmentContext ToContext();
    }
}
