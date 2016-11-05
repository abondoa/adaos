using Adaos.Common.Extenders;
using Adaos.Shell.Core;
using Adaos.Shell.Interface.Execution;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Adaos.Shell.Library.AdHoc
{
    public class ScopeEnvironment : BaseEnvironment
    {
        public override string Name { get; }

        public ScopeEnvironment(string name) : base(true)
        {
            Name = name;
        }

        public override Command Retrieve(string commandName)
        {
            int temp;
            if (int.TryParse(commandName, out temp))
            {
                return args => { return new[] { new DummyArgument(commandName) }.Then(args.Flatten()); };
            }
            else
            {
                return base.Retrieve(commandName);
            }
        }
    }
}
