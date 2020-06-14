using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.Types;
using Utilities.Extensions;

namespace Vectors.Extensions
{
    public static class IntIntervalEx
    {
        public static bool Contains(this IEnumerable<IntInterval> intervals, int point, bool strict = false)
        {
            return intervals.Any(i => i.Contains(point, strict));
        }

        /// <summary>
        /// Shifts all intervals so that <paramref name="intervals"/>[0].From gets equal to 0
        /// </summary>
        /// <param name="intervals"></param>
        /// <returns></returns>
        public static IEnumerable<IntInterval> ShiftToZero(this IEnumerable<IntInterval> intervals)
        {
            var offset = intervals.FirstItem().From;
            return intervals.Select(i => i.Sub(offset));
        }
    }
}
