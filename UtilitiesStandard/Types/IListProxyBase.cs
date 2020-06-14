using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Utilities.Types
{
    public class IListProxyBase<T> : IList<T>
    {
        protected readonly IList<T> _base;

        public virtual T this[int index]
        {
            get => _base[index];
            set => _base[index] = value;
        }
        public virtual int Count => _base.Count;
        public virtual bool IsReadOnly => _base.IsReadOnly;

        public IListProxyBase(IList<T> @base)
        {
            _base = @base ?? throw new ArgumentNullException(nameof(@base));
        }

        public virtual void Add(T item)
        {
            _base.Add(item);
        }

        public virtual void Clear()
        {
            _base.Clear();
        }

        public virtual bool Contains(T item)
        {
            return _base.Contains(item);
        }

        public virtual void CopyTo(T[] array, int arrayIndex)
        {
            _base.CopyTo(array, arrayIndex);
        }

        public virtual IEnumerator<T> GetEnumerator()
        {
            return _base.GetEnumerator();
        }

        public virtual int IndexOf(T item)
        {
            return _base.IndexOf(item);
        }

        public virtual void Insert(int index, T item)
        {
            _base.Insert(index, item);
        }

        public virtual bool Remove(T item)
        {
            return _base.Remove(item);
        }

        public virtual void RemoveAt(int index)
        {
            _base.RemoveAt(index);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
