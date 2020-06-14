using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Utilities.Extensions
{
    public class FindResult<T>
    {
        readonly Lazy<T> _Value = new Lazy<T>();

        public bool Found { get; }
        public int Index { get; }
        public T Value
        {
            get
            {
                return Found 
                    ? _Value.Value 
                    : throw new InvalidOperationException("The value has not been found.");
            }
        }
        public T ValueOrDefault
        {
            get
            {
                return Found
                    ? _Value.Value
                    : default;
            }
        }

        /// <summary>
        /// Nothing is found
        /// </summary>
        public FindResult()
        {
            Index = -1;
            _Value = default;
        }
        public FindResult(int index, T value)
        {
            checkIndex(index);
            Index = index;
            _Value = new Lazy<T>(() => value);
            Found = true;
        }
        /// <summary>
        /// If finding algorithm does not provide value, it can be found automatically, when user
        /// accessing to the <see cref="Value"/> property by enumerating <paramref name="valueSource"/> 
        /// till known <see cref="Index"/> value.
        /// </summary>
        /// <param name="index"></param>
        /// <param name="valueSource"></param>
        public FindResult(int index, IEnumerable<T> valueSource)
        {
            checkIndex(index);
            Index = index;
            _Value = new Lazy<T>(() => valueSource.First((v, i) => i == Index).Value);
            Found = true;
        }

        FindResult(int index, T value, bool found)
        {
            Index = index;
            _Value = new Lazy<T>(() => value);
            Found = found;
        }

        void checkIndex(int index)
        {
            if (index < 0)
            {
                throw new ArgumentOutOfRangeException("Index must be in range from 0 to Inf");
            }
        }

        public FindResult<TResult> Cast<TResult>(Func<T, TResult> valueCaster)
        {
            
            return new FindResult<TResult>(Index, valueCaster(_Value.Value), Found);
        }
    }
}