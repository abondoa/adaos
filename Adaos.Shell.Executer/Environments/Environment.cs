using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adaos.Shell.Interface;
using Adaos.Shell.Executer.CachedEnumerable;
using Adaos.Shell.Executer.Environments.AdHocEnvironments;

namespace Adaos.Shell.Executer.Environments
{
    abstract public class Environment : IEnvironment
    {
        private Dictionary<string, Command> _nameToCommandDictionary;
        private bool _allowUnbinding;
        private IDictionary<string,IEnvironment> _childEnvs;


        public Environment(bool allowUnbinding = false)
        {
            _nameToCommandDictionary = new Dictionary<string, Command>();
            _allowUnbinding = allowUnbinding;
            _childEnvs = new Dictionary<string,IEnvironment>();
            Identifier = new EnvironmentUniqueIdentifier(Name);
        }

        abstract public string Name
        {
            get;
        }

        private IEnumerable<IArgument> _commandWrapper(Command inner, IEnumerable<IArgument> args)
        {
            return new CachedEnumerable<IArgument>(inner(args));
        }

        public virtual void Bind(string commandName, Command command)
        {
            Command actualCommand = x => _commandWrapper(command,x);
            _nameToCommandDictionary.Add(commandName.ToLower(), actualCommand);
        }

        public virtual Command Retrieve(string commandName)
        {
            if (_nameToCommandDictionary.Keys.Contains(commandName.ToLower()))
            {
                return _nameToCommandDictionary[commandName.ToLower()];
            }
            Command result = null;
            foreach (var env in ChildEnvironments)
            {
                result = env.Retrieve(commandName);
                if (result != null)
                {
                    break;
                }
            }
            if (result == null)
            { 
                if(commandName == Name)
                {
                    result = _environmentCommand;
                }
            }
            return result;
        }

        public static IEnumerable<IEnvironment> operator +(IEnumerable<IEnvironment> container, Environment addition)
        {
            foreach (var item in container)
            {
                yield return item;
            }
            yield return addition;
        }

        public virtual void Bind(Command command, params string[] commandNames)
        {
            if (commandNames.Count() < 1)
            {
                throw new Exception("There must be at least one name for the function to be bound to");
            }
            foreach (var commandName in commandNames)
            {
                Bind(commandName, command);
            }
        }


        public IEnumerable<string> Commands
        {
            get { return _nameToCommandDictionary.Select(x => x.Key).OrderBy(x => x); }
        }

        public virtual void UnBind(string commandName)
        {
            if (AllowUnbinding)
            {
                _nameToCommandDictionary.Remove(commandName);
            }
            else
            {
                throw new NotSupportedException("This environment ("+Name+") does not support unbinding of commands");
            }
        }

        public bool AllowUnbinding
        {
            get { return _allowUnbinding; }
        }

        public IEnvironmentUniqueIdentifier Identifier
        {
            get;
            private set;
        }

        public virtual IEnumerable<IEnvironmentUniqueIdentifier> Dependencies
        {
            get { yield break; }
        }


        public IEnumerable<IEnvironment> ChildEnvironments
        {
            get
            {
                lock (_childEnvs)
                {
                    foreach (var env in _childEnvs)
                    {
                        yield return env.Value;
                    }
                }
            }
        }


        public void AddEnvironment(IEnvironment environment)
        {
            lock (_childEnvs)
            {
                if (_childEnvs.Keys.Contains(environment.Name))
                {
                    throw new ArgumentException("Unably to add environment: '" + environment + "', it is already a child of " + Name);
                }
                _childEnvs.Add(environment.Name,new EnvironmentContext(environment,this));
            }
        }

        public void RemoveEnvironment(IEnvironment environment)
        {
            lock (_childEnvs)
            {
                if (!_childEnvs.Keys.Contains(environment.Name))
                {
                    throw new ArgumentException("Unably to remove environment: '" + environment + "', it is not a child of " + Name);
                }
                _childEnvs.Remove(environment.Name);
            }
        }

        private IEnumerable<IArgument> _environmentCommand(IEnumerable<IArgument> args)
        {
            if (args.FirstOrDefault() == null)
            {
                throw new SemanticException(-1,"Environment-command '" + this.Name + "' received no command name as first argument");
            }
            foreach(var res in Retrieve(args.First().Value)(args.Skip(1)))
            {
                yield return res;
            }
        }
    }
}
