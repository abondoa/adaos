using System.Collections.Generic;

namespace Adaos.Shell.Interface.Execution
{
    /// <summary>
    /// An interface describing the environment context, used to put an Adaos environment into 
    /// a specific context. 
    /// </summary>
    public interface IEnvironmentContext : IEnvironment
    {
        /// <summary>
        /// Get the full name of the context - i.e. the short names of all ancestors and the current context
        /// </summary>
        IEnumerable<string> EnvironmentNames { get; }

        /// <summary>
        /// Is the inner environment enabled for command execution?
        /// </summary>
		bool IsEnabled { get; set; }

        IEnvironmentContext Parent { get; }

		/// <summary>
		/// Enumerates the child environments of this environment
		/// </summary>
        IEnumerable<IEnvironmentContext> ChildEnvironments { get; }

		/// <summary>
		/// Finds a single child envionment based on its short-name
		/// </summary>
		/// <param name="childEnvironmentName"></param>
		/// <returns></returns>
        IEnvironmentContext ChildEnvironment(string childEnvironmentName);

        /// <summary>
        /// The child environment will be encapsulated in an IEnvironementContext and become part of the ChildEnvironments
        /// </summary>
        /// <param name="environment">The environment to be added</param>
        /// <returns>The created context</returns>
        IEnvironmentContext AddChild(IEnvironment environment);

        /// <summary>
        /// Recursively remove the given child and all its decendants
        /// </summary>
        /// <param name="environment"></param>
        void RemoveChild(IEnvironmentContext environment);

        /// <summary>
        /// The inner IEnvironment being encapsulated
        /// </summary>
        IEnvironment Inner { get; }
		
		/// <summary>
		/// Gets the environment-command.
		/// </summary>
		/// <value>The environment-command.</value>
		Command EnvironmentCommand { get;}
    }
}
