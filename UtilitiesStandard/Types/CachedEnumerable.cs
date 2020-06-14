using System;
using System.Collections.Generic;
using System.Collections;

namespace Utilities.Types
{
    /// <summary>
    /// Not thread safe (even IEnumerator)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class CachedEnumerableDecorator<T> : Disposable, IReadOnlyList<T>
    {
        readonly IEnumerator<T> _source;
        readonly IList<T> _cache = new List<T>();
        readonly bool isAlreadyCached;

        public T this[int index]
        {
            get
            {
                throwIfDisposed();

                if (!isAlreadyCached)
                {
                    var dI = index - _cache.Count;
                    while (dI >= 0 && moveNextAndUpdate())
                    {
                        dI--;
                        _cache.Add(_source.Current);
                    }
                    if (dI >= 0)
                    {
                        throw new InvalidOperationException($"Underlying Enumerator didn't provide element with index {index}");
                    }
                }

                return _cache[index];
            }
        }

        public int Count
        {
            get
            {
                throwIfDisposed();

                if (!isAlreadyCached)
                {
                    while (moveNextAndUpdate())
                    {
                        _cache.Add(_source.Current);
                    }
                }

                return _cache.Count;
            }
        }

        public bool IsEnumerated { get; private set; }

        public CachedEnumerableDecorator(IEnumerable<T> source)
        {
            _source = source.GetEnumerator();

            if (source is IList<T>)
            {
                _cache = (IList<T>)source;
                isAlreadyCached = true;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            throwIfDisposed();

            foreach (var item in _cache)
            {
                yield return item;
            }
            if (!isAlreadyCached)
            {
                while (moveNextAndUpdate())
                {
                    _cache.Add(_source.Current);
                    yield return _source.Current;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throwIfDisposed();

            return GetEnumerator();
        }

        bool moveNextAndUpdate()
        {
            var result = _source.MoveNext();
            IsEnumerated = !result;
            if (IsEnumerated && !isAlreadyCached)
            {
                ((List<T>)_cache).TrimExcess();
            }

            return result;
        }

        protected override void DisposeManagedState()
        {
            _source.Dispose();
        }
    }
}
