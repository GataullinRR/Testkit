using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Collections;

namespace Utilities.Types
{
    public abstract class QueryableProxyBase<T> : IQueryable<T>
    {
        readonly IQueryable<T> _base;

        public virtual Type ElementType => _base.ElementType;
        public virtual Expression Expression => _base.Expression;
        public virtual IQueryProvider Provider => _base.Provider;
        
        public QueryableProxyBase(IQueryable<T> @base)
        {
            _base = @base ?? throw new ArgumentNullException(nameof(@base));
        }

        public virtual IEnumerator<T> GetEnumerator() => _base.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_base).GetEnumerator();
    }
}
