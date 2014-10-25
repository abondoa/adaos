using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adaos.Common.Enumerable
{
    internal class CachedEnumerator<T> : IEnumerator<T>
    {
        int _counter;
        CachedEnumerable<T> _cachedEnumerable;
        public CachedEnumerator(CachedEnumerable<T> argNum)
        {
            _cachedEnumerable = argNum;
            Reset();
        }

        public T Current
        {
            get { return _cachedEnumerable[_counter]; }
        }

        public void Dispose()
        {
            _cachedEnumerable = null;
        }

        object System.Collections.IEnumerator.Current
        {
            get { return Current; }
        }

        public bool MoveNext()
        {
            return _cachedEnumerable.HasElement(++_counter);
        }

        public void Reset()
        {
            _counter = -1;
        }
    }
}
