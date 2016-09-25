using Adaos.Shell.Core;
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

        public ScopeEnvironment(string name)
        {
            Name = name;
        }
    }
}
