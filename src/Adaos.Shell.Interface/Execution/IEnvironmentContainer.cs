using System.Collections.Generic;

namespace Adaos.Shell.Interface.Execution
{
    /// <summary>
    /// An interface describing a container used to hold environments. 
    /// </summary>
    public interface IEnvironmentContainer
    {
        /// <summary>
        /// Get the loaded environments ordered by promotions. Promoted environments first.
        /// </summary>
        IEnumerable<IEnvironmentContext> EnabledEnvironments { get; }

        /// <summary>
        /// Get the unloaded environments ordered by promotions. Promoted environments first.
        /// </summary>
        IEnumerable<IEnvironmentContext> DisabledEnvironments { get; }

        /// <summary>
        /// Load the given environment into the container at the root.
        /// </summary>
        /// <param name="environment">The environment to load.</param>
        /// <returns>The context created around the <paramref name="environment"/></returns>
        IEnvironmentContext LoadEnvironment(IEnvironment environment);

        /// <summary>
        /// Load given environment into this container as a child of <paramref name="parent"/>.
        /// </summary>
        /// <param name="environment">The environment that will be loaded</param>
        /// <param name="parent">The environment context underwhich the new environment is loaded</param>
        /// <returns>The context created around the <paramref name="environment"/></returns>
        IEnvironmentContext LoadEnvironment(IEnvironment environment, IEnvironmentContext parent);

        /// <summary>
        /// Unload the given environment from the container at the root.
        /// </summary>
        /// <param name="environment">The environment to unload.</param>
        void UnloadEnvironment(IEnvironment environment);

        /// <summary>
        /// Unload given environment from this container and as a child of <paramref name="parent"/>.
        /// </summary>
        /// <param name="environment">The environment that will be unloaded</param>
        /// <param name="parent">The environment context underwhich the new environment is unloaded</param>
        void UnloadEnvironment(IEnvironment environment, IEnvironmentContext parent);

        /// <summary>
        /// Move context to the front of contexts when getting loaded/unloaded environments.
        /// </summary>
        /// <param name="context">The context to move to the front.</param>
        void PromoteEnvironment(IEnvironmentContext context);

        /// <summary>
        /// Move context to the back of contexts when getting loaded/unloaded environments
        /// </summary>
        /// <param name="context">The context to to the back.</param>
        void DemoteEnvironment(IEnvironmentContext context);
    }

    public static class EnvironmentContainerExtender
    {
        public static void LoadEnvironments(this IEnvironmentContainer self, IEnumerable<IEnvironment> environments)
        {
            foreach(var environment in environments)
            {
                self.LoadEnvironment(environment);
            }
        }
    }
}
