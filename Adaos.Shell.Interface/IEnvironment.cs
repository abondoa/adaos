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
        void AddEnvironment(IEnvironment environment);
        void RemoveEnvironment(IEnvironment environment);
    }

    public static class EnvironmentExtender
    { 
        public static IEnumerable<IEnvironment> DecendentEnvironments(this IEnvironment self)
        {
            foreach (var child in self.ChildEnvironments)
            {
                foreach (var subEnv in child.FamilyEnvironments())
                {
                    yield return subEnv;
                }
            }
        }

        public static IEnumerable<IEnvironment> FamilyEnvironments(this IEnvironment self)
        {
            yield return self;
            foreach (var subEnv in self.DecendentEnvironments())
            {
                yield return subEnv;
            }
        }
    }
}
