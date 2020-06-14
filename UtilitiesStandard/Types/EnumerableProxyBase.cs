using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Utilities.Types
{
    public abstract class EnumerableProxyBase<T> : IEnumerable<T>
    {
        readonly IEnumerable<T> _base;

        public EnumerableProxyBase(IEnumerable<T> @base)
        {
            _base = @base ?? throw new ArgumentNullException(nameof(@base));
        }

        public IEnumerator<T> GetEnumerator() => _base.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_base).GetEnumerator();
    }
}
