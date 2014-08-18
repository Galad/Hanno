using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hanno.Collection
{
    public class DoubleList<T> : IList<T>
    {
        private readonly List<T> _list;
        private readonly List<T> _secondaryList;

        public DoubleList()
        {
            _list = new List<T>();
            _secondaryList = new List<T>();
        }

        public int IndexOf(T item)
        {
            return List.IndexOf(item);
        }

        public void Insert(int index, T item)
        {
            if (index <= _list.Count)
            {
                _list.Insert(index, item);
            }
            else if (index - _list.Count <= SecondaryList.Count)
            {
                SecondaryList.Insert(index - _list.Count, item);
            }
            else
            {
                throw new ArgumentOutOfRangeException("index", "Index is out of range");
            }
        }

        public void RemoveAt(int index)
        {
            _list.RemoveAt(index);
        }

        public T this[int index]
        {
            get
            {
                return List[index];
            }
            set
            {
                List[index] = value;
            }
        }

        public void Add(T item)
        {
            _list.Add(item);
        }

        public void Clear()
        {
            _list.Clear();
            SecondaryList.Clear();
        }

        public bool Contains(T item)
        {
            return List.Contains(item);
        }

        public void CopyTo(T[] array, int arrayIndex)
        {
            List.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return List.Count; }
        }

        public bool IsReadOnly
        {
            get { return ((ICollection<T>)_list).IsReadOnly; }
        }

        public bool Remove(T item)
        {
            return _list.Remove(item) || SecondaryList.Remove(item);
        }

        public IEnumerator<T> GetEnumerator()
        {
            return List.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return List.GetEnumerator();
        }

        public IList<T> SecondaryList
        {
            get { return _secondaryList; }
        }

        protected List<T> List
        {
            get { return _list.Concat(SecondaryList).ToList(); }
        }
    }
}
