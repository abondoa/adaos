using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Adaos.Shell.Interface;
using Adaos.Shell.Core.Extenders;
using Adaos.Common.Extenders;
using Adaos.Shell.Interface.Execution;
using Adaos.Shell.Library.Standard;
using Adaos.Shell.Execution.Environments;

namespace Adaos.Shell.Execution
{
    class EnvironmentContainer : IEnvironmentContainer
    {
        IList< IEnvironmentContext> _innerList;
        private IEnvironmentContext _rootEnvironment;

        public EnvironmentContainer(IEnumerable<IEnvironment> environments)
        {
            _rootEnvironment = new Environments.RootEnvironment();
            _rootEnvironment.AddChildren(
                (new SystemEnvironment()).ToEnum<IEnvironment>().
                Then(environments));

            _innerList = new List<IEnvironmentContext>(_rootEnvironment.ChildEnvironments.Select(x => x.FamilyEnvironments()).Flatten());
        }

        public IEnumerable<IEnvironmentContext> LoadedEnvironments
        {
            get { return _innerList.Where(x => x.IsEnabled); }
        }

        public IEnumerable<IEnvironmentContext> UnloadedEnvironments
        {
            get { return _innerList.Where(x => !x.IsEnabled); }
        }

        public void LoadEnvironment(IEnvironment environment)
        {
            _rootEnvironment.AddChild(environment);
            var contextAdded = _rootEnvironment.ChildEnvironments.FirstOrDefault(x => x.Inner == environment);
            if (contextAdded == null)
            {
                throw new ArgumentException("Trying to remove unknown environment '" + environment + "'");
            }
            _innerList.Add(contextAdded);
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
