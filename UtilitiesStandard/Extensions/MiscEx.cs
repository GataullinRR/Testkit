using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Utilities;
using Utilities.Extensions;
using Utilities.Types;

namespace Utilities.Extensions
{
    public static class MiscEx
    {
        public static async Task<Match> MatchAsync(this Regex regex, string str)
        {
            return await Task.Run(() => regex.Match(str));
        }

        public static IValueProvider<T> GetProvider<T>(this T value)
        {
            return new ValueProvider<T>(value);
        }

        public static T GetTargetOrDefault<T>(this WeakReference<T> value)
            where T : class
        {
            var has = value.TryGetTarget(out T target);

            return has
                ? target
                : null;
        }
    }
}
