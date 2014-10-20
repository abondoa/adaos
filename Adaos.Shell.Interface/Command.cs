using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adaos.Shell.Interface
{
    public delegate IEnumerable<IArgument> Command(IEnumerable<IArgument> arguments);
}
