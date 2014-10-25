using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adaos.Shell.Interface
{
    public delegate IEnumerable<IArgument> Command(params IEnumerable<IArgument>[] arguments);
    public delegate IEnumerable<IArgument> SimpleCommand(IEnumerable<IArgument> arguments);
    public delegate IEnumerable<IArgument> ArgumentLookupCommand(IArgumentValueLookup lookup, params IEnumerable<IArgument>[] arguments);
}
