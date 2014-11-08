using System;
using System.Collections.Generic;

namespace Adaos.Shell.Interface
{
    public interface IEnvironmentContext : IEnvironment
    {
        IEnumerable<string> EnvironmentNames { get; }
		bool IsEnabled { get; set; }

		/// <summary>
		/// Enumerates the child environments of this environment
		/// </summary>
		IEnumerable<IEnvironment> ChildEnvironments { get; }

		/// <summary>
		/// Finds a single child envionment based on its short-name
		/// </summary>
		/// <param name="childEnvironmentName"></param>
		/// <returns></returns>
		IEnvironment ChildEnvironment(string childEnvironmentName);


		void AddEnvironment(IEnvironment environment);
		void RemoveEnvironment(IEnvironment environment);
    }
}
