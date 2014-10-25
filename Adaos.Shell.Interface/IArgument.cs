using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adaos.Shell.Interface
{
    public interface IArgument
    {
        string Name { get; }
        string Value { get; }
        bool ToExecute { get; }
        int Position { get; }
        bool HasName { get; }
    }
}
