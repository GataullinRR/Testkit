using System;
using System.Collections.Generic;
using Utilities;
using Utilities.Extensions;
using Utilities.Types;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities
{
    public static class TestingUtils
    {
        public static void AreEqual<T>(Action<bool, string, object[]> assertor, IEnumerable<T> expected, IEnumerable<T> actual)
        {
            AreEqual((cond, msg) => assertor(cond, msg, null), expected, actual);
        }
        public static void AreEqual<T>(Action<bool, string> assertor, IEnumerable<T> expected, IEnumerable<T> actual)
        {
            Func<T, T, bool> comparer = (v1, v2) => ((IEqualityComparer<T>)EqualityComparer<T>.Default).Equals(v1, v2);
            var result = ArrayUtils.Compare(expected, actual, comparer);

            assertor(result.IsMatch, result.ToString());
        }
    }
}
