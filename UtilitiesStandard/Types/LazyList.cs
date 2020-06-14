using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.Extensions;
using Utilities.Types;

namespace Utilities.Types
{
    public class LazyList<T> : IEnumerable<T>
    {
        readonly IEnumerable<T> _source;
        IEnumerator<T> _sourceEnumerator;
        readonly List<T> _list = new List<T>();

        public T this[int index]
        {
            get
            {
                createEnumeratorIfNotExist();

                while (_list.Count <= index)
                {
                    if (!tryTakeOne())
                    {
                        throw new IndexOutOfRangeException();
                    }
                }

                return _list[index];

                void createEnumeratorIfNotExist()
                {
                    if (_sourceEnumerator == null)
                    {
                        _sourceEnumerator = _source.GetEnumerator();
                    }
                }
            }
        }

        public LazyList(IEnumerable<T> source)
        {
            _source = source;
        }
        public LazyList(params Func<T>[] functions)
        {
            _source = wrapToLazyExecutable();

            return;

            IEnumerable<T> wrapToLazyExecutable()
            {
                foreach (var f in functions)
                {
                    yield return f();
                }
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            foreach (var item in _list)
            {
                yield return item;
            }

            while (tryTakeOne())
            {
                yield return _list.LastElement();
            }
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        bool tryTakeOne()
        {
            var hasOne = _sourceEnumerator.MoveNext();
            if (hasOne)
            {
                _list.Add(_sourceEnumerator.Current);
            }

            return hasOne;
        }
    }
}
