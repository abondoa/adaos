using System.Collections.Generic;
using System.Linq;
using Adaos.Shell.Interface;
using Adaos.Shell.Interface.Execution;

namespace Adaos.Shell.Core.Extenders
{
    public static class EnvironmentExtender
    {
        public static IEnumerable<IEnvironmentContext> DecendentEnvironments(this IEnvironmentContext self)
        {
            foreach (var child in self.ChildEnvironments)
            {
                foreach (var subEnv in child.FamilyEnvironments())
                {
                    yield return subEnv;
                }
            }
        }

        public static IEnumerable<IEnvironmentContext> FamilyEnvironments(this IEnvironmentContext self)
        {
            yield return self;
            foreach (var subEnv in self.DecendentEnvironments())
            {
                yield return subEnv;
            }
        }

        public static void AddChildren(this IEnvironmentContext self, IEnumerable<IEnvironment> innerEnvironments)
        {
            foreach (var env in innerEnvironments)
            {
                self.AddChild(env);
            }
        }

        public static void RemoveChildren(this IEnvironmentContext self, IEnumerable<IEnvironmentContext> innerEnvironments)
        {
            foreach (var env in innerEnvironments)
            {
                self.RemoveChild(env);
            }
        }
    }
}
