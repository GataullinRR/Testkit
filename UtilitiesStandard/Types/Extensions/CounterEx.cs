using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.Types;
using Utilities.Extensions;

namespace Utilities.Extensions
{
    public static class CounterEx
    {
        public static Counter Merge(this IEnumerable<Counter> counters, Counter.MergeMode mergeMode)
        {
            return Counter.Merge(counters, mergeMode);
        }
    }
}
