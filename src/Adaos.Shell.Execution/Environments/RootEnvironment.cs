using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Adaos.Shell.Interface;

namespace Adaos.Shell.Execution.Environments
{
    class RootEnvironment : IEnvironmentContext
    {
        private IList< IEnvironment> _childEnvs;

        public RootEnvironment()
        {
            _childEnvs = new List<IEnvironment>();
        }

        public string Name
        {
            get { throw new InvalidOperationException("Never read the Name of the root environment"); }
        }

        public void Bind(Command command, params string[] commandNames)
        {
            throw new InvalidOperationException("Unable to bind commands to the root environment");
        }

        public Command Retrieve(string commandName)
        {
            throw new InvalidOperationException("Unable to retrieve a command from the root environment");
        }

        public IEnumerable<string> Commands
        {
            get { throw new InvalidOperationException("Unable to get commands from the root environment"); }
        }

        public void Unbind(string commandName)
        {
            throw new InvalidOperationException("Unable to unbind commands to the root environment");
        }

        public bool AllowUnbinding
        {
            get { return false; }
        }

        public IEnumerable<Type> Dependencies
        {
            get { yield return this.GetType(); }
        }

        public IEnumerable<IEnvironment> ChildEnvironments
        {
            get 
            {
                foreach (var child in _childEnvs)
                {
                    yield return child;
                }
            }
        }

        public void AddEnvironment(IEnvironment environment)
        {
            _childEnvs.Add(environment);
        }

        public void RemoveEnvironment(IEnvironment environment)
        {
            _childEnvs.Remove(environment);
        }


        public IEnvironment ChildEnvironment(string childEnvironmentName)
        {
            return _childEnvs.Single(x => x.Name == childEnvironmentName);
        }


        public IEnvironmentContext ToContext()
        {
            return this;
        }

        public string QualifiedName(string separator)
        {
            throw new InvalidOperationException("Never get the QualifiedName of the root environment");
        }

        public IEnumerable<string> EnvironmentNames
        {
            get { throw new InvalidOperationException("Never read the EnvironmentNames of the root environment"); }
        }
    }
}
