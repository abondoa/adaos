using System;
using System.Linq;
using Adaos.Shell.Interface;
using System.Collections.Generic;

namespace ModuleA
{
    public class ModuleA : IModule
    {
        public System.Collections.Generic.IEnumerable<IEnvironment> Environments
        {
            get { yield return new TestEnv(); }
        }

        public string Name
        {
            get { return "modulea"; }
        }
    }

    public class TestEnv : IEnvironment
    {
        public bool AllowUnbinding
        {
            get { return false; }
        }

        public void Bind(Command command, params string[] commandNames)
        {
            throw new NotImplementedException();
        }

        public System.Collections.Generic.IEnumerable<string> Commands
        {
            get { yield return "test"; yield return "counter"; }
        }

        public System.Collections.Generic.IEnumerable<Type> Dependencies
        {
            get { yield break; }
        }

        public bool IsRedoable(Command command)
        {
            return true;
        }

        public string Name
        {
            get { return "test"; }
        }

        public Command Retrieve(string commandName)
        {
            if (commandName == "test")
            {
                return x => x.Aggregate((y,z)=> y.Union(z));
            }
            if (commandName == "counter")
            {
                return Counter;
            }

            return null;
        }

        private IEnumerable<IArgument> Counter(params IEnumerable<IArgument>[] args)
        {
            for (int i = 0; i <= Math.Pow(2,20) ; i++)
            {
                //for (int j = 0; j <= Math.Pow(2, 31); j++)
                {
                }
                yield return new Argument(i.ToString());
            }
        }

        public void Unbind(string commandName)
        {
            return;
        }

        public void AddEnvironment(IEnvironment environment)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IEnvironment> ChildEnvironments
        {
            get { yield break; }
        }

        public void RemoveEnvironment(IEnvironment environment)
        {
            return;
        }

        public IEnvironment ChildEnvironment(string childEnvironmentName)
        {
            throw new NotImplementedException();
        }

        public IEnvironmentContext AsContext()
        {
            return new EnvironmentContext(this, null);
        }

        public string QualifiedName(string separator)
        {
            return Name;
        }
    }

    internal class EnvironmentContext : IEnvironmentContext
    {
        private IEnvironmentContext parrent;
        private IEnvironment env;

        public EnvironmentContext(IEnvironment env, IEnvironmentContext parrent)
        {
            this.env = env;
            this.parrent = parrent;
        }

        public bool AllowUnbinding => false;

        public IEnumerable<IEnvironmentContext> ChildEnvironments { get { yield break; } }

        public IEnumerable<string> Commands => env.Commands;

        public IEnumerable<Type> Dependencies => env.Dependencies;

        public Command EnvironmentCommand
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public IEnumerable<string> EnvironmentNames
        {
            get
            {
                if (parrent != null)
                    foreach (var e in parrent.EnvironmentNames)
                        yield return e;

                yield return env.Name;
            }
        }

        public IEnvironment Inner => env;

        public bool IsEnabled
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public string Name => env.Name;

        public IEnvironmentContext Parent => parrent;

        public IEnvironmentContext AddChild(IEnvironment environment)
        {
            throw new NotImplementedException();
        }

        public IEnvironmentContext AsContext()
        {
            return this;
        }

        public void Bind(Command command, params string[] commandNames)
        {
            throw new NotImplementedException();
        }

        public IEnvironmentContext ChildEnvironment(string childEnvironmentName)
        {
            throw new NotImplementedException();
        }

        public string QualifiedName(string separator)
        {
            return string.Join(separator, EnvironmentNames);
        }

        public void RemoveChild(IEnvironmentContext environment)
        {
            throw new NotImplementedException();
        }

        public Command Retrieve(string commandName)
        {
            return env.Retrieve(commandName);
        }

        public void Unbind(string commandName)
        {
            env.Unbind(commandName);
        }
    }

    public class Argument : IArgument
    {
        public int Position
        {
            get { return -1; }
        }

        public bool ToExecute
        {
            get { return false; }
        }

        public string Value
        {
            get;
            private set;
        }

        public Argument(string val)
        {
            Value = val;
        }

        public string Name
        {
            get { throw new NotImplementedException(); }
        }

        public bool HasName
        {
            get { return false; }
        }

        public bool Equals(IArgument other)
        {
            return HasName == other.HasName &&
                (HasName || Name.Equals(other.Name)) &&
                Position == other.Position &&
                ToExecute == other.ToExecute &&
                Value.Equals(other.Value);
        }
    }
}