using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adaos.Shell.Interface;

namespace Adaos.Shell.Executer.Environments.AdHocEnvironments
{
    internal class EnvironmentContext : IEnvironment
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="inner"></param>
        /// <param name="parent"></param>
        /// <param name="separator"></param>
        public EnvironmentContext(IEnvironment inner, IEnvironment parent, string separator = null)
        {
            Inner = inner;
            Parent = parent;
            Separator = separator;
        }

        public string Name
        {
            get 
            {
                if(Parent != null)
                {
                    return Parent.Name + '.' + Inner.Name;
                }
                return Inner.Name;
            }
        }

        public IEnvironmentUniqueIdentifier Identifier
        {
            get { return Inner.Identifier; }
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

        public void UnBind(string commandName)
        {
            Inner.UnBind(commandName);
        }

        public bool AllowUnbinding
        {
            get { return Inner.AllowUnbinding; }
        }

        public IEnumerable<IEnvironmentUniqueIdentifier> Dependencies
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

        public IEnvironment Parent 
        {
            get; 
            private set; 
        }

        public string Separator 
        {
            get; 
            private set; 
        }
    }
}
