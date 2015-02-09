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
        /// Get the long-name of the environment with ancestors.
        /// Used as a qualifier for the environment.
        /// </summary>
        /// <param name="separator">Used to separate the environment levels, the standard is "."</param>
        /// <returns>A string composed of the names of the environment and ancestors separated by the given separator.</returns>
        string QualifiedName(string separator);
        
        /// <summary>
        /// Bind a new command to the environment.
        /// </summary>
        /// <param name="command">The executable command to bind.</param>
        /// <param name="commandNames">An array of names to bind the command to.</param>
        void Bind(Command command, params string[] commandNames);

        /// <summary>
        /// Find a command based on its name.
        /// </summary>
        /// <param name="commandName">The name of the command to find.</param>
        /// <returns>The command refered to by the given name or null if not found.</returns>
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
        /// <param name="commandName">The name of the command to unbind.</param>
        void Unbind(string commandName);

        /// <summary>
        /// Get whether this environment allows commands to be unbound. 
        /// </summary>
        bool AllowUnbinding { get; }

        /// <summary>
        /// Enumerates the environments, which this environment need to function.
        /// </summary>
        IEnumerable<Type> Dependencies { get; }

        /// <summary>
        /// Converts the environment into an IEnvironmentContext or returns self if it is a an IEnvironmentContext.
        /// </summary>
        IEnvironmentContext AsContext();
    }
}
