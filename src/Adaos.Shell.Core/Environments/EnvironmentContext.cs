using System;
using System.Collections.Generic;
using System.Linq;
using Adaos.Shell.Interface;

namespace Adaos.Shell.Core.Environments
{
    internal class EnvironmentContext : IEnvironmentContext
    {
        bool _enabled = true;
        private IDictionary<string, IEnvironmentContext> _childEnvs;

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
            _childEnvs = new Dictionary<string, IEnvironmentContext>();
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

        public IEnumerable<IEnvironmentContext> ChildEnvironments
        {
            get
            {
                lock (_childEnvs)
                {
                    return _childEnvs.Select(x => x.Value);
                }
            }
        }
        public IEnvironmentContext AddChild(IEnvironment environment)
        {
            lock (_childEnvs)
            {
                if (_childEnvs.Keys.Contains(environment.Name))
                {
                    throw new ArgumentException("Unably to add environment: '" + environment + "', it is already a child of '" + this + "'");
                }
                var result = new EnvironmentContext(environment,this);
                _childEnvs.Add(environment.Name, result);
                return result;
            }
        }

        public void RemoveChild(IEnvironmentContext environment)
        {
            lock (_childEnvs)
            {
                if (!_childEnvs.Keys.Contains(environment.Name))
                {
                    throw new ArgumentException("Unably to remove environment: '" + environment + "', it is not a child of '" + this + "'");
                }
                _childEnvs.Remove(environment.Name);
            }
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

        public IEnvironmentContext AsContext()
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


        public bool IsEnabled
        {
            get
            {
                return _enabled;
            }
            set
            {
                _enabled = value;
                foreach (var child in ChildEnvironments)
                {
                    child.AsContext().IsEnabled = value;
                }
            }
        }

        public IEnvironmentContext ChildEnvironment(string childEnvironmentName)
        {
            lock (_childEnvs)
            {
                if (!_childEnvs.Keys.Contains(childEnvironmentName))
                {
                    throw new ArgumentException("Unably to find environment: '" + childEnvironmentName + "', it is not a child of '" + this + "'");
                }
                return _childEnvs[childEnvironmentName];
            }
        }
    }
}
