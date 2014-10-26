using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Adaos.Shell.Interface;

namespace Adaos.Shell.Execution
{
    class EnvironmentContainer : IDictionary<string[],IEnvironmentContext>
    {
        IDictionary<string[], IEnvironmentContext> _innerDictionary;
        public EnvironmentContainer()
        {
            _innerDictionary = new Dictionary<string[], IEnvironmentContext>();
        }

        public void Add(string[] key, IEnvironmentContext value)
        {
            _innerDictionary.Add(key, value);
        }

        public bool ContainsKey(string[] key)
        {
            return _innerDictionary.ContainsKey(key);
        }

        public ICollection<string[]> Keys
        {
            get { return _innerDictionary.Keys; }
        }

        public bool Remove(string[] key)
        {
            return _innerDictionary.Remove(key);
        }

        public bool TryGetValue(string[] key, out IEnvironmentContext value)
        {
            return _innerDictionary.TryGetValue(key, out value);
        }

        public ICollection<IEnvironmentContext> Values
        {
            get { return _innerDictionary.Values; }
        }

        public IEnvironmentContext this[params string[] key]
        {
            get
            {
                return _innerDictionary[key];
            }
            set
            {
                _innerDictionary[key] = value;
            }
        }

        public void Add(KeyValuePair<string[], IEnvironmentContext> item)
        {
            _innerDictionary.Add(item);
        }

        public void Clear()
        {
            _innerDictionary.Clear();
        }

        public bool Contains(KeyValuePair<string[], IEnvironmentContext> item)
        {
            return _innerDictionary.Contains(item);
        }

        public void CopyTo(KeyValuePair<string[], IEnvironmentContext>[] array, int arrayIndex)
        {
            _innerDictionary.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _innerDictionary.Count; }
        }

        public bool IsReadOnly
        {
            get { return _innerDictionary.IsReadOnly; }
        }

        public bool Remove(KeyValuePair<string[], IEnvironmentContext> item)
        {
            return _innerDictionary.Remove(item);
        }

        public IEnumerator<KeyValuePair<string[], IEnvironmentContext>> GetEnumerator()
        {
            return _innerDictionary.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (_innerDictionary as System.Collections.IEnumerable).GetEnumerator();
        }
    }
}
