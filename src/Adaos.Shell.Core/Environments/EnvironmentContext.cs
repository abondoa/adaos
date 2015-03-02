using System;
using System.Collections.Generic;
using System.Linq;
using Adaos.Shell.Interface;
using Adaos.Shell.Interface.Exceptions;
using Adaos.Common.Extenders;
using Adaos.Shell.Core.Extenders;
using Adaos.Shell.Interface.Execution;
using Adaos.Shell.Interface.SyntaxAnalysis;

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

		private IEnumerable<IArgument> _environmentCommand(params IEnumerable<IArgument>[] args)
		{
			if (args[0].FirstOrDefault() == null)
			{
				throw new SemanticException(-1,"Environment-command '" + this.Name + "' received no command name as first argument");
			}
			IArgument firstArg = args.First ().First ();
			var command = Retrieve (firstArg.Value);
			if(command == null)
			{
				var childEnv = this.ChildEnvironments.FirstOrDefault (x => x.Inner.Name == firstArg.Value);
				if (childEnv == null) 
				{
					throw new SemanticException (-1, "Environment-command '" + this.Name + "' unable to find command '" + args [0].First ().Value + "'");
				} 
				command = childEnv.EnvironmentCommand;
			}
			foreach(var res in command(args[0].Skip(1).Then(args.Skip(1).Flatten())))
			{
				yield return res;
			}
		}

        public Command Retrieve(string commandName)
        {
			if(commandName == Inner.Name)
			{
				return _environmentCommand;
			}
			var childEnv = ChildEnvironments.FirstOrDefault (x => x.Inner.Name == commandName);
			if (childEnv != null) 
			{
				return childEnv.EnvironmentCommand;
			}
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

		public Command EnvironmentCommand {get {return _environmentCommand;}}
    }
}
