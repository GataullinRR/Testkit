using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Utilities.Extensions;

namespace Utilities.Types
{
    public class ListBase<T, TCollection> : IList<T>
        where TCollection : IList<T>
    {
        protected readonly TCollection _baseCollection;

        public virtual T this[int index]
        {
            get => _baseCollection[index];
            set => _baseCollection[index] = value;
        }

        public virtual int Count => _baseCollection.Count;

        public virtual bool IsReadOnly => throw new NotImplementedException();

        public ListBase(TCollection baseCollection)
        {
            _baseCollection = baseCollection;
        }

        public ListBase<T, TCollection> Add(params T[] value)
        {
            return Add((IEnumerable<T>)value);
        }
        public ListBase<T, TCollection> Add(IEnumerable<T> values)
        {
            foreach (var value in values)
            {
                _baseCollection.Add(value);
            }

            return this;
        }
        public virtual void Add(T item)
        {
            _baseCollection.Add(item);
        }

        public virtual void Clear()
        {
            _baseCollection.Clear();
        }

        public virtual bool Contains(T item)
        {
            return _baseCollection.Contains(item);
        }

        public virtual void CopyTo(T[] array, int arrayIndex)
        {
            _baseCollection.CopyTo(array, arrayIndex);
        }

        public virtual IEnumerator<T> GetEnumerator()
        {
            return _baseCollection.GetEnumerator();
        }

        public virtual int IndexOf(T item)
        {
            return _baseCollection.IndexOf(item);
        }

        public virtual void Insert(int index, T item)
        {
            _baseCollection.Insert(index, item);
        }

        public virtual bool Remove(T item)
        {
            return _baseCollection.Remove(item);
        }

        public virtual void RemoveAt(int index)
        {
            _baseCollection.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _baseCollection.GetEnumerator();
        }
    }
}
