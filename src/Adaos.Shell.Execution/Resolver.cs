using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adaos.Shell.Interface;
using Adaos.Shell.Core.Extenders;
using Adaos.Common.Extenders;
using Adaos.Shell.Execution.Exceptions;

namespace Adaos.Shell.Execution
{
    class Resolver : IResolver
    {
        public Resolver()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="command"></param>
        /// <param name="environments"></param>
        /// <returns></returns>
        public Command Resolve(
            IExecution command,
            IEnumerable<IEnvironmentContext> environments)
        {
            Command result = null;
            if (command.EnvironmentNames.Count() > 0)
            {
                IEnvironmentContext env = null;
                IList<IEnvironmentContext> visitedEnvs = new List<IEnvironmentContext>();
                foreach (var envName in command.EnvironmentNames)
                {
                    env = environments.FirstOrDefault(x => x.Name.ToLower().Equals(envName.ToLower()));

                    if (env == null)
                    {
                        if (visitedEnvs.Count > 0)
                        {
                            throw new VMException(command.Position, "Environment: '" + envName + "' was not found in environment: '" + visitedEnvs.Select(x => x.Name).Aggregate((x, y) => x + "." + y) + "'");
                        }
                        else
                        {
                            throw new VMException(command.Position, "Environment: '" + envName + "' was not found");
                        }
                    }
                    visitedEnvs.Add(env);
                    environments = env.ChildEnvironments;
                }

                result = env.Retrieve(command.CommandName);
                if (result == null)
                {
					throw new VMException(command.Position, "Command: '" + command.CommandName + "' was not found in environment: '" + env.Name + "'");
                }
            }
            else
            {
                foreach (var env in environments)
                {
                    result = env.Retrieve(command.CommandName);
                    if (result != null)
                    {
                        break;
                    }
                }
                if (result == null)
                {
                    throw new VMException(command.Position, "Command: '" + command.CommandName + "' was not found in any environment");
                }
            }

            return result;
        }

        public IEnvironment GetEnvironmentOf (
            IExecution command,
            IEnumerable<IEnvironmentContext> environments)
        {
            Command result = null;
            if (command.EnvironmentNames.Count() > 0)
            {
                IEnvironmentContext env = null;
                IList<IEnvironmentContext> visitedEnvs = new List<IEnvironmentContext>();
                foreach (var envName in command.EnvironmentNames)
                {
                    env = environments.FirstOrDefault(x => x.Name.ToLower().Equals(envName.ToLower()));

                    if (env == null)
                    {
                        if (visitedEnvs.Count > 0)
                        {
                            throw new VMException(command.Position, "Environment: '" + envName + "' was not found in environment: '" + visitedEnvs.Select(x => x.Name).Aggregate((x, y) => x + "." + y) + "'");
                        }
                        else
                        {
                            throw new VMException(command.Position, "Environment: '" + envName + "' was not found");
                        }
                    }
                    visitedEnvs.Add(env);
                    environments = env.ChildEnvironments;
                }

                result = env.Retrieve(command.CommandName);
                if (result == null)
                {
                    throw new VMException(command.Position, "Command: '" + command.CommandName + "' was not found in environment: '" + env.Name + "'");
                }
                return env;
            }
            else
            {
                foreach (var env in environments)
                {
                    result = env.Retrieve(command.CommandName);
                    if (result != null)
                    {
                        return env;
                    }
                }
            }

            throw new VMException(command.Position, "Command: '" + command.CommandName + "' was not found in any environment");
        }
    }
}
