using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adaos.Shell.Interface
{
    /// <summary>
    /// An interface describing the Adaos environment, used to encapsulate commands and behaviours.
    /// </summary>
    public interface IEnvironment
    {
        /// <summary>
        /// Get the name of the environment.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// The long-name of the environment with ancestors
        /// </summary>
        /// <param name="separator">Used to separate the environment levels, the standard is "."</param>
        /// <returns></returns>
        string QualifiedName(string separator);
        
        /// <summary>
        /// Bind a new command to the environment
        /// </summary>
        /// <param name="command"></param>
        /// <param name="commandNames"></param>
        void Bind(Command command, params string[] commandNames);

        /// <summary>
        /// Find a command based on its name
        /// </summary>
        /// <param name="commandName"></param>
        /// <returns>The command refered to by the given name or null if not found</returns>
        Command Retrieve(string commandName);

        /// <summary>
        /// Get a collection of commands associated with this enviroment. 
        /// </summary>
        IEnumerable<string> Commands { get; }

        /// <summary>
        /// Remove the binding of a given command name in this environment. 
        /// Does nothing, is no command is bound with the given name. 
        /// TODO: Check up on whether this is true.
        /// </summary>
        /// <param name="commandName"></param>
        void Unbind(string commandName);

        /// <summary>
        /// Get whether this environment allows commands to be unbound. 
        /// </summary>
        bool AllowUnbinding { get; }

        /// <summary>
        /// Enumerates the environments, which this environment need to function
        /// </summary>
        IEnumerable<Type> Dependencies { get; }

        /// <summary>
        /// Converts the environment into an IEnvironmentContext or returns self if it is a an IEnvironmentContext
        /// </summary>
        /// <returns></returns>
        IEnvironmentContext AsContext();
    }
}
