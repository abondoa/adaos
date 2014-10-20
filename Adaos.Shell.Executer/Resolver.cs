using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adaos.Shell.Interface;
using Adaos.Shell.SyntaxAnalysis.Exceptions;

namespace Adaos.Shell.Executer
{
    class Resolver
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
            ICommand command, 
            IEnumerable<IEnvironment> environments)
        {
            Command result = null;
            if (command.EnvironmentNames.Count() > 0)
            {
                IEnvironment env = null;
                IList<IEnvironment> visitedEnvs = new List<IEnvironment>();
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
                    throw new VMException(command.Position, "Command: '" + command.CommandName + "' was not found in environment: '" + command.EnvironmentName + "'");
                }
            }
            else
            {
                foreach (var env in environments.Select(x => x.FamilyEnvironments()).Aggregate((x,y) => x.Union(y)))
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
            ICommand command,
            IEnumerable<IEnvironment> environments)
        {
            Command result = null;
            if (command.EnvironmentNames.Count() > 0)
            {
                IEnvironment env = null;
                IList<IEnvironment> visitedEnvs = new List<IEnvironment>();
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
                    throw new VMException(command.Position, "Command: '" + command.CommandName + "' was not found in environment: '" + command.EnvironmentName + "'");
                }
                return env;
            }
            else
            {
                foreach (var env in environments.Select(x => x.FamilyEnvironments()).Aggregate((x, y) => x.Union(y)))
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
