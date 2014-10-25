using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Adaos.Common.Interface;

namespace Adaos.Common.Enumerable
{
    internal class CachedEnumerable<T> : ICachedEnumerable<T>
    {
        IList<T> _cachedResults;
        IEnumerator<T> _resultEnumerator;

        public CachedEnumerable(IEnumerable<T> enumerable)
        {
            _cachedResults = new List<T>();
            _resultEnumerator = enumerable.GetEnumerator();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return new CachedEnumerator<T>(this);
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private bool _transfer(int num = 1)
        {
            if (_resultEnumerator == null)
            { // No more results from inner enum
                return false;
            }
            for (int i = 0; i < num; ++i)
            {
                if (!_resultEnumerator.MoveNext())
                {
                    _resultEnumerator.Dispose();
                    _resultEnumerator = null; // No more results from inner enum
                    return false;
                }
                _cachedResults.Add(_resultEnumerator.Current);
            }
            return true;
        }

        public bool HasElement(int index)
        {
            if (index >= _cachedResults.Count)
            {
                return _transfer(index - _cachedResults.Count + 1);
            }
            return true;
        }

        public T this[int index]
        {
            get
            {
                if (HasElement(index))
                {
                    return _cachedResults[index];
                }
                return default(T);
            }
        }
    }
}
