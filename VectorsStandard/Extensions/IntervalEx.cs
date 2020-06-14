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
    public static class IntervalEx
    {
        public static Interval GetValuesInterval(this IEnumerable<double> values)
        {
            return new Interval(values.Min(), values.Max());
        }
    }
}
