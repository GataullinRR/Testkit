using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.Extensions;
using Utilities.Types;

namespace Utilities.Types
{
    public class FComparer<T> : IEqualityComparer<T>
    {
        public static implicit operator FComparer<T>(Func<T, T, bool> comparer)
        {
            return new FComparer<T>(comparer);
        }

        readonly Func<T, T, bool> _comparer;
        public FComparer(Func<T, T, bool> comparer)
        {
            _comparer = comparer;
        }

        bool IEqualityComparer<T>.Equals(T x, T y)
        {
            return _comparer(x, y);
        }

        int IEqualityComparer<T>.GetHashCode(T obj)
        {
            return obj.GetHashCode();
        }
    }
}
