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
        /// Get the unique identifier of the environment.
        /// </summary>
        IEnvironmentUniqueIdentifier Identifier { get; }

        /// <summary>
        /// Bing a command to this environment.
        /// </summary>
        /// <param name="command">The executable command to bind.</param>
        /// <param name="commandNames">An array of names to bind the command to.</param>
        void Bind(Command command, params string[] commandNames);

        /// <summary>
        /// Retrieve a command from the environment from the given command name.
        /// </summary>
        /// <param name="commandName">The name bound to the command being retrieved.</param>
        /// <returns>A command if one is bound with the given name. Otherwise returns null.</returns>
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
        /// Get a collection of identifiers for enviroments that this environment depends on.
        /// </summary>
        IEnumerable<IEnvironmentUniqueIdentifier> Dependencies { get; }

        /// <summary>
        /// Get a collection of child environments.
        /// </summary>
        IEnumerable<IEnvironment> ChildEnvironments { get; }

        /// <summary>
        /// Add an environment as a child to this environment.
        /// </summary>
        /// <param name="environment">The child environment to add.</param>
        void AddEnvironment(IEnvironment environment);

        /// <summary>
        /// Remove a child environment from this environment. 
        /// </summary>
        /// <param name="environment">The child environment to remove.</param>
        void RemoveEnvironment(IEnvironment environment);
    }

    /// <summary>
    /// A static extension class for the <see cref="IEnvironment"/> interface.
    /// </summary>
    public static class EnvironmentExtender
    { 
        /// <summary>
        /// Get a collection of sub enviroments of this enviroments 
        /// (this includes child environments, grand-child environemnts and so on).
        /// </summary>
        /// <param name="self"></param>
        /// <returns>A collection of all sub environments to this environment.</returns>
        public static IEnumerable<IEnvironment> DescendentEnvironments(this IEnvironment self)
        {
            foreach (var child in self.ChildEnvironments)
            {
                foreach (var subEnv in child.FamilyEnvironments())
                {
                    yield return subEnv;
                }
            }
        }

        /// <summary>
        /// Get a collection of all environments in the family of this environment.
        /// That includes this environment and all descending environments, 
        /// but does not include parents of this environment.
        /// </summary>
        /// <param name="self"></param>
        /// <returns>A collection of all the environments of this family.</returns>
        public static IEnumerable<IEnvironment> FamilyEnvironments(this IEnvironment self)
        {
            yield return self;
            foreach (var subEnv in self.DescendentEnvironments())
            {
                yield return subEnv;
            }
        }
    }
}
