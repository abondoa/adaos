using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adaos.Shell.Executer.CachedEnumerable
{
    public class CachedEnumerator<T> : IEnumerator<T> where T : class
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
            if (_cachedEnumerable[_counter + 1] == null)
            {
                return false;
            }
            _counter++;
            return true;
        }

        public void Reset()
        {
            _counter = -1;
        }
    }
}
