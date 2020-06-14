using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection.Emit;
using System.Text;
using Utilities;
using Utilities.Extensions;
using Utilities.Types;

namespace Utilities
{
    public static class ParsingUtils
    {
        public static IPEndPoint TryParseIPEndPoint(string ipEndPoint)
        {
            var ipAndPort = ipEndPoint.Split(":");
            if (ipAndPort.Length < 2)
            {
                return null;
            }
            else
            {
                var ipOk = IPAddress.TryParse(ipAndPort[0], out IPAddress ip);
                var portOk = ushort.TryParse(ipAndPort[1], out ushort port);
                return ipOk && portOk
                    ? new IPEndPoint(ip, port)
                    : null;
            }
        }

        public static List<double> ParseDoubles(string doubles, char separator)
        {
            return doubles.Split(new[] { separator }, StringSplitOptions.RemoveEmptyEntries)
                .Select(double.Parse)
                .ToList();
        }


        delegate bool InvariantTryParseDelegate<T>(string str, out T value);
        delegate bool TryParseDelegate<T>(string str, NumberStyles ns, IFormatProvider fp, out T value);
        public static Func<string, T?> GetInvariantParser<T>()
            where T : struct
        {
            if (!typeof(T).IsPrimitive)
            {
                throw new NotSupportedException();
            }

            switch (typeof(T).Name)
            {
                case "Byte":
                    return wrap<byte>(byte.TryParse);
                case "SByte":
                    return wrap<sbyte>(sbyte.TryParse);
                case "Int32":
                    return wrap<int>(int.TryParse);
                case "UInt32":
                    return wrap<uint>(uint.TryParse);
                case "Int16":
                    return wrap<short>(short.TryParse);
                case "UInt16":
                    return wrap<ushort>(ushort.TryParse);
                case "Int64":
                    return wrap<long>(long.TryParse);
                case "UInt64":
                    return wrap<ulong>(ulong.TryParse);
                case "Single":
                    return wrap<float>(float.TryParse);
                case "Double":
                    return wrap<double>(double.TryParse);
                case "Decimal":
                    return wrap<decimal>(decimal.TryParse);
                case "Boolean":
                    return wrapInvariant<bool>(bool.TryParse);
                case "Char":
                    return str => str.Length == 1 ? (T?)(object)str[0] : null;
                default:
                    throw new NotSupportedException();
            }

            //////////////////////////////////////////

            Func<string, T?> wrapInvariant<TD>(InvariantTryParseDelegate<TD> parser)
                where TD : struct
            {
                return str =>
                {
                    var success = parser(str, out TD value);
                    return success ? (T?)(object)value : null;
                };
            }
            Func<string, T?> wrap<TD>(TryParseDelegate<TD> parser)
                where TD : struct
            {
                return str =>
                {
                    var success = parser(str, NumberStyles.Any, CultureInfo.InvariantCulture, out TD value);
                    return success ? (T?)(object)value : null;
                };
            }
        }
    }
}
