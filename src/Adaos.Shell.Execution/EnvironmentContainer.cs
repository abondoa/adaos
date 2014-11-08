using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Adaos.Shell.Interface;

namespace Adaos.Shell.Execution
{
    class EnvironmentContainer : IList<IEnvironmentContext>
    {
        IList< IEnvironmentContext> _innerList;
        public EnvironmentContainer()
        {
			_innerList = new List<IEnvironmentContext>();
        }
		
		public EnvironmentContainer(IEnumerable<IEnvironmentContext> envContexts)
		{
			_innerList = new List<IEnvironmentContext>(envContexts);
		}

        public void Add(IEnvironmentContext value)
        {
            _innerList.Add(value);
        }

        public bool Remove(IEnvironmentContext key)
        {
            return _innerList.Remove(key);
        }

        public void Clear()
        {
            _innerList.Clear();
        }

        public bool Contains(IEnvironmentContext item)
        {
            return _innerList.Contains(item);
        }

        public void CopyTo(IEnvironmentContext[] array, int arrayIndex)
        {
            _innerList.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return _innerList.Count; }
        }

		public IEnvironmentContext this[int index]
		{
			get {return _innerList [index];}
			set {_innerList [index] = value;}
		}

		public bool IsReadOnly {get {return _innerList.IsReadOnly;}}

		public void RemoveAt(int index)
		{
			_innerList.RemoveAt (index);
		}

		public int IndexOf(IEnvironmentContext item)
		{
			return _innerList.IndexOf (item);
		}

		public void Insert(int index, IEnvironmentContext item)
		{
			_innerList.Insert (index, item);
		}

        public IEnumerator<IEnvironmentContext> GetEnumerator()
        {
            return _innerList.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return (_innerList as System.Collections.IEnumerable).GetEnumerator();
        }
    }
}
