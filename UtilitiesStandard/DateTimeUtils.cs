using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities;
using Utilities.Extensions;
using Utilities.Types;

namespace Utilities
{
    public static class DateTimeUtils
    {
        public static bool IsSameDay(params DateTime[] days)
        {
            var id = getDayId(days[0]);
            foreach (var d in days)
            {
                if (getDayId(d) != id)
                {
                    return false;
                }
            }

            return true;

            int getDayId(DateTime dt)
            {
                return dt.DayOfYear + dt.Year * 366;
            }
        }
    }
}
