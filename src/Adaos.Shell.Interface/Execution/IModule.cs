using System.Collections.Generic;

namespace Adaos.Shell.Interface.Execution
{
    /// <summary>
    /// An interface describing an Adaos module, used to group environment as a package.
    /// </summary>
    public interface IModule
    {
        /// <summary>
        /// Enumerates the environments of this module.
        /// </summary>
        IEnumerable<IEnvironment> Environments { get; }

        /// <summary>
        /// Get the name of the module.
        /// </summary>
        string Name { get; }
    }
}
