using System;
using System.Collections.Generic;
namespace Adaos.Shell.Interface
{
    public interface IValueLookup<TArgumentType> : IEnumerable<KeyValuePair<string, TArgumentType>>
    {
        TArgumentType this[string name] { get; }
    }

    public interface IArgumentValueLookup : IValueLookup<IArgument>
    {
    }
}
