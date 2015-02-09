using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adaos.Shell.Interface
{
    public interface IEnvironmentContainer
    {
        /// <summary>
        /// The loaded environments ordered by promotions. Promoted environments first.
        /// </summary>
        IEnumerable<IEnvironmentContext> LoadedEnvironments { get; }

        /// <summary>
        /// The unloaded environments ordered by promotions. Promoted environments first.
        /// </summary>
        IEnumerable<IEnvironmentContext> UnloadedEnvironments { get; }

        /// <summary>
        /// Load environment at root.
        /// </summary>
        /// <param name="environment"></param>
        void LoadEnvironment(IEnvironment environment);

        /// <summary>
        /// Unload environment at root.
        /// </summary>
        /// <param name="environment"></param>
        void UnloadEnvironment(IEnvironment environment);

        /// <summary>
        /// Move context to the front of contexts when getting loaded/unloaded environments
        /// </summary>
        /// <param name="context"></param>
        void PromoteEnvironment(IEnvironmentContext context);

        /// <summary>
        /// Move context to the back of contexts when getting loaded/unloaded environments
        /// </summary>
        /// <param name="context"></param>
        void DemoteEnvironment(IEnvironmentContext context);
    }
}
