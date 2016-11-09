using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Adaos.Shell.Interface;
using Adaos.Shell.Core.Extenders;
using Adaos.Common.Extenders;
using Adaos.Shell.Library.Standard;
using Adaos.Shell.Execution.Environments;
using Adaos.Shell.Interface.Execution;

namespace Adaos.Shell.Execution
{
    class EnvironmentContainer : IEnvironmentContainer
    {
        IList< IEnvironmentContext> _innerList;
        private IEnvironmentContext _rootEnvironment;

        public EnvironmentContainer()
        {
            _rootEnvironment = new RootEnvironment();
            _innerList = new List<IEnvironmentContext>();
        }

        public EnvironmentContainer(IEnumerable<IEnvironment> environments)
        {
            _rootEnvironment = new RootEnvironment();
            _rootEnvironment.AddChildren(
                (new SystemEnvironment()).ToEnum<IEnvironment>().
                Then(environments));

            _innerList = new List<IEnvironmentContext>(_rootEnvironment.ChildEnvironments.Select(x => x.FamilyEnvironments()).Flatten());
        }

        public IEnumerable<IEnvironmentContext> EnabledEnvironments
        {
            get { return _innerList.Where(x => x.IsEnabled); }
        }

        public IEnumerable<IEnvironmentContext> DisabledEnvironments
        {
            get { return _innerList.Where(x => !x.IsEnabled); }
        }

        public IEnvironmentContext LoadEnvironment(IEnvironment environment)
        {
            _rootEnvironment.AddChild(environment);
            IEnvironmentContext contextAdded;
            if (environment is IEnvironmentContext)
                contextAdded = environment as IEnvironmentContext;
            else
                contextAdded = _rootEnvironment.ChildEnvironments.FirstOrDefault(x => x.Inner == environment);
            if (contextAdded == null)
            {
                throw new ArgumentException("Failed to load environment '" + environment + "'");
            }
            _innerList.Add(contextAdded);
            foreach (var decendent in contextAdded.DecendentEnvironments())
            {
                _innerList.Add(decendent);
            }
            return contextAdded;
        }

        public IEnvironmentContext LoadEnvironment(IEnvironment environment, IEnvironmentContext parent)
        {
            IEnvironmentContext contextAdded = parent.AddChild(environment);
            if (contextAdded == null)
            {
                throw new ArgumentException($"Failed to load environment '{environment}'");
            }
            _innerList.Add(contextAdded);
            foreach(var decendent in contextAdded.DecendentEnvironments())
            {
                _innerList.Add(decendent);
            }
            return contextAdded;
        }

        public void UnloadEnvironment(IEnvironment environment)
        {
            var contextToRemove = _rootEnvironment.ChildEnvironments.FirstOrDefault(x => x.Inner == environment);
            if(contextToRemove == null)
            {
                throw new ArgumentException("Trying to remove unknown environment '"+environment+"'");
            }
            _rootEnvironment.RemoveChild(contextToRemove);
            _innerList.Remove(contextToRemove);
        }

        public void UnloadEnvironment(IEnvironment environment, IEnvironmentContext parent)
        {
            IEnvironmentContext contextToRemove;
            if (environment is IEnvironmentContext)
                contextToRemove = environment as IEnvironmentContext;
            else
                contextToRemove = parent.ChildEnvironments.FirstOrDefault(x => x.Inner == environment);
            if (contextToRemove == null)
            {
                throw new ArgumentException($"Trying to remove unknown environment '{environment}' from parent '{parent}'");
            }
            parent.RemoveChild(contextToRemove);
            _innerList.Remove(contextToRemove);
        }

        public void PromoteEnvironment(IEnvironmentContext context)
        {
            _innerList.Remove(context);
            _innerList.Insert(0, context);
        }

        public void DemoteEnvironment(IEnvironmentContext context)
        {
            _innerList.Remove(context);
            _innerList.Add(context);
        }
    }
}
