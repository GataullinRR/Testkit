using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Extensions
{
    public static class EnumeratorEx
    {
        public static T GetNextOrThrow<T>(this IEnumerator<T> enumerator)
        {
            enumerator.MoveNext();

            return enumerator.Current;
        }
    }
}
