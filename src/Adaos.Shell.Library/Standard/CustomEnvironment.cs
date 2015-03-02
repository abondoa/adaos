using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adaos.Shell.Core;
using Adaos.Shell.Interface;
using Adaos.Shell.Interface.Execution;

namespace Adaos.Shell.Library.Standard
{
    public class CustomEnvironment : BaseEnvironment
    {
        private IDictionary<string, IList<Type>> _dependencies;
        public CustomEnvironment() : base(true)
        {
            _dependencies = new Dictionary<string, IList<Type>>();
        }

        public override string Name
        {
            get { return "custom"; }
        }

        public void Bind(string commandName, Command command, IEnumerable<Type> dependencies)
        {
            base.Bind(commandName, command);
            _dependencies.Add(commandName, dependencies.ToList());
        }

        public override void Unbind(string commandName)
        {
            base.Unbind(commandName);
            _dependencies.Remove(commandName);
        }

        public override IEnumerable<Type> Dependencies
        {
            get
            {
                if (_dependencies.Count > 0)
                {
                    var dependencies = _dependencies.Select(x => x.Value).Aggregate((x, y) => { return x.Union(y).ToList(); });
                    foreach (var dep in dependencies)
                    {
                        yield return dep;
                    }
                }

                yield break;
            }
        }
    }
}
