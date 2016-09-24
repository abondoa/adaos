using System.Collections.Generic;
using Adaos.Shell.Interface.SyntaxAnalysis;
using System.Linq;

namespace Adaos.Shell.Interface
{
    /// <summary>
    /// An interface describing a value lookup table, 
    /// used by the Adaos execution system to pass arguments to executing commands.
    /// </summary>
    /// <typeparam name="TArgumentType">The type of the argument node that holds the value.</typeparam>
    public interface IValueLookup<TArgumentType> : IEnumerable<KeyValuePair<string, TArgumentType>>
    {
        /// <summary>
        /// Get a value by argument name.
        /// </summary>
        /// <param name="name">The name of the argument holding the value.</param>
        /// <returns></returns>
        TArgumentType this[string name] { get; }

        /// <summary>
        /// Get a lookup table mapping the arguments to values.
        /// </summary>
        ILookup<string, TArgumentType> Lookup { get; }
    }

    /// <summary>
    /// An interface describing an <see cref="IArgument"/> value lookup table,
    /// used by the Adaos execution system to pass arguments to executing commands.
    /// </summary>
    public interface IArgumentValueLookup : IValueLookup<IArgument>
    { }
}
