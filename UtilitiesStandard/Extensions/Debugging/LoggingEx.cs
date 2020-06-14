using System;
using System.Collections.Generic;
using Utilities;
using Utilities.Extensions;
using Utilities.Types;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Extensions.Debugging
{
    public static class LoggingEx
    {
        public static T Dump<T>(this T obj, object name)
        {
            obj.ToString().Dump(name);

            return obj;
        }
        public static double Dump(this double obj, object name, int round)
        {
            obj.Round(round).Dump(name);

            return obj;
        }
        public static double Dump(this double obj, object name)
        {
            obj.ToStringInvariant().Dump(name);

            return obj;
        }
        public static T Dump<T>(this T obj)
        {
            obj.ToString().Dump();

            return obj;
        }

        public static string Dump(this string obj, object name)
        {
            $"{name.ToString()}: {obj.ToString()}".Dump();

            return obj;
        }
        public static string Dump(this string line)
        {
            Console.WriteLine(line);

            return line;
        }
    }
}
