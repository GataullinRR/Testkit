using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.Extensions;

namespace Utilities.Types
{
    public enum Comparasion : int
    {
        A_GREATER = 1,
        B_GREATER = -1,
        EQUAL = 0
    }

    public class IComparerAction<T> : IComparer<T>
    {
        readonly Func<T, T, int> _comparer;

        public IComparerAction(Func<T, T, Comparasion> comparer)
        {
            _comparer = (x, y) => (int)comparer(x, y);
        }
        public IComparerAction(Func<T, T, int> comparer)
        {
            _comparer = comparer;
        }

        public int Compare(T x, T y)
        {
            return _comparer(x, y);
        }
    }
}
