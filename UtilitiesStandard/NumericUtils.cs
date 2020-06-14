using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities.Extensions;

namespace Utilities
{
    public static class NumericUtils
    {
        public static T Max<T>(params T[] vals)
        {
            return vals.Max();
        }
        public static T Min<T>(params T[] vals)
        {
            return vals.Min();
        }
    }
}
