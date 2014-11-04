using System;
using System.Collections.Generic;
using Adaos.Shell.Interface;

namespace Adaos.Shell.Core.Environments
{
    internal class EnvironmentContext : IEnvironmentContext
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inner"></param>
        /// <param name="parent"></param>
        /// <param name="separator"></param>
        public EnvironmentContext(IEnvironment inner, IEnvironmentContext parent = null)
        {
            Inner = inner;
            Parent = parent;
        }

        public string Name
        {
            get
            {
                return Inner.Name;
            }
        }

        public void Bind(Command command, params string[] commandNames)
        {
            Inner.Bind(command, commandNames);
        }

        public Command Retrieve(string commandName)
        {
            return Inner.Retrieve(commandName);
        }

        public IEnumerable<string> Commands
        {
            get { return Inner.Commands; }
        }

        public void Unbind(string commandName)
        {
            Inner.Unbind(commandName);
        }

        public bool AllowUnbinding
        {
            get { return Inner.AllowUnbinding; }
        }

        public IEnumerable<Type> Dependencies
        {
            get { return Inner.Dependencies; }
        }

        public IEnumerable<IEnvironment> ChildEnvironments
        {
            get
            {
                foreach (var child in Inner.ChildEnvironments)
                {
                    yield return new EnvironmentContext(child, this);
                }
            }
        }

        public void AddEnvironment(IEnvironment environment)
        {
            Inner.AddEnvironment(environment);
        }

        public void RemoveEnvironment(IEnvironment environment)
        {
            Inner.RemoveEnvironment(environment);
        }

        public IEnvironment Inner
        {
            get;
            private set;
        }

        public IEnvironmentContext Parent
        {
            get;
            private set;
        }

        public IEnvironment ChildEnvironment(string childEnvironmentName)
        {
            return Inner.ChildEnvironment(childEnvironmentName);
        }


        public IEnvironmentContext ToContext()
        {
            return this;
        }

        public string QualifiedName(string separator)
        {
            if (Parent != null)
            {
                return Parent.QualifiedName(separator) + separator + Inner.Name;
            }
            return Inner.Name;
        }

        public IEnumerable<string> EnvironmentNames
        {
            get
            {
                if (Parent != null)
                {
                    foreach (var name in Parent.EnvironmentNames)
                    {
                        yield return name;
                    }
                }
                yield return Name;
            }
        }
    }
}
