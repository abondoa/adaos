using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Adaos.Shell.Executer.CachedEnumerable
{
    public class CachedEnumerable<T> : IEnumerable<T> where T : class
    {
        IList<T> _cachedResults;
        IEnumerator<T> _resultEnumerator;
        int _size;

        public CachedEnumerable(IEnumerable<T> enumerable)
        {
            _cachedResults = new List<T>();
            _resultEnumerator = enumerable.GetEnumerator();
            _size = 0;
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
            for (int i = 0; i < num; ++i)
            {
                if (!_resultEnumerator.MoveNext())
                {
                    _resultEnumerator.Dispose();
                    return false;
                }
                _cachedResults.Add(_resultEnumerator.Current);
                _size++;
            }
            return true;
        }

        public T this[int index]
        {
            get
            {
                if (index >= _size)
                {
                    if (!_transfer(index - _size + 1))
                    {
                        return null;
                    }
                }
                return _cachedResults[index];
            }
        }
    }
}
