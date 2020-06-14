using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.Extensions;

namespace Utilities.Types
{
    /// <summary>
    /// IEnumerable which supports adding by utilizing <see cref="Enumerable.Count{TSource}(IEnumerable{TSource})"/>.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Enumerable<T> : IEnumerable<T>
    {
        public static implicit operator Enumerable<T>(T[] sequence)
        {
            return new Enumerable<T>().Add(sequence);
        }
        public static implicit operator Enumerable<T>(List<T> sequence)
        {
            return new Enumerable<T>().Add(sequence);
        }

        IEnumerable<T> _values;

        public Enumerable()
        {
            _values = Enumerable.Empty<T>();
        }

        public Enumerable<T> Add(T value)
        {
            return Add(new T[] { value });
        }
        public Enumerable<T> Add(IEnumerable<T> value)
        {
            _values = _values.Concat(value);

            return this;
        }

        public Enumerable<T> AddRepeatedly(T value, int count)
        {
            return Add(Enumerable.Repeat(value, count));
        }
        public Enumerable<T> AddRepeatedly(IEnumerable<T> value, int count)
        {
            while (count-- > 0)
            {
                _values = _values.Concat(value);
            }

            return this;
        }

        public IEnumerator<T> GetEnumerator()
        {
            return _values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _values.GetEnumerator();
        }
    }
}
