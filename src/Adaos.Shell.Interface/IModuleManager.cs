using Adaos.Shell.Interface.Execution;

namespace Adaos.Shell.Interface
{
    /// <summary>
    /// An interface describing a manager for Adaos modules, that serves as packages of environments.
    /// </summary>
    public interface IModuleManager
    {
        /// <summary>
        /// Get an instance of a module, by a given filename.
        /// </summary>
        /// <param name="fileName">The name of the file containing the module.</param>
        /// <returns>An instance of a <see cref="IModule"/> from the given file.</returns>
        IModule GetInstance(string fileName, IVirtualMachine virtualMachine);
    }
}
