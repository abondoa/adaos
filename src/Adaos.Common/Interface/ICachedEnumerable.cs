using System.Collections.Generic;

namespace Adaos.Common.Interface
{
    /// <summary>
    /// An interface used to indicate an enumerable, that when iterated multiple times, will only execute the inner enumerator once.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface ICachedEnumerable<T> : IEnumerable<T>
    {
    }
}
