using System.Collections.Generic;
using System.Linq;
using Adaos.Shell.Interface;

namespace Adaos.Shell.Core.Extenders
{
    public static class EnvironmentExtender
    {
        public static IEnumerable<IEnvironmentContext> DecendentEnvironments(this IEnvironmentContext self)
        {
            foreach (var child in self.ChildEnvironments.Select(x => new Environments.EnvironmentContext(x, self)))
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

        public static IEnumerable<IEnvironmentContext> DecendentEnvironments(this IEnvironment self)
        {
            return self.ToContext().DecendentEnvironments();
        }

        public static IEnumerable<IEnvironmentContext> FamilyEnvironments(this IEnvironment self)
        {
            return self.ToContext().FamilyEnvironments();
        }

        public static void AddEnvironments(this IEnvironment self, params IEnvironment[] innerEnvironments)
        {
            foreach (var env in innerEnvironments)
            {
                self.AddEnvironment(env);
            }
        }

        public static void RemoveEnvironments(this IEnvironment self, params IEnvironment[] innerEnvironments)
        {
            foreach (var env in innerEnvironments)
            {
                self.RemoveEnvironment(env);
            }
        }
    }
}
