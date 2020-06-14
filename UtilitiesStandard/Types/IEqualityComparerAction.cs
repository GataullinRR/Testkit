using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.Extensions;

namespace Utilities.Types
{
    public class IEqualityComparerAction<T> : IEqualityComparer<T>
    {
        readonly Func<T, T, bool> _comparer;
        readonly Func<T, int> _getHashCode;

        public IEqualityComparerAction(Func<T, T, bool> comparer)
            : this(comparer, _ => 0)
        {

        }
        public IEqualityComparerAction(Func<T, T, bool> comparer, Func<T, int> getHashCode)
        {
            _comparer = comparer;
            _getHashCode = getHashCode;
        }

        public bool Equals(T x, T y)
        {
            return _comparer(x, y);
        }

        public int GetHashCode(T obj)
        {
            return _getHashCode(obj);
        }
    }
}
