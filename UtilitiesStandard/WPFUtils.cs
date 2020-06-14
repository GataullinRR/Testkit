using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using Utilities;
using Utilities.Extensions;
using Utilities.Types;

namespace Utilities
{
    public static class WPFUtils
    {
        public static readonly Func<string, bool> DoubleValidator = s => s.TryParseToDoubleInvariant().HasValue;
        public static readonly Func<string, bool> ShortValidator = s => s.TryParseToInt16Invariant().HasValue;
        public static readonly Func<string, bool> UShortValidator = s => s.TryParseToUInt16Invariant().HasValue;

        public static void SetThenNotify<T>(ref T propBackingField, T value,  
            PropertyChangedEventHandler handler, object sender, [CallerMemberName]string propertyName = "")
        {
            propBackingField = value;
            handler?.Invoke(sender, new PropertyChangedEventArgs(propertyName));
        }   

        public static Func<string, bool> GetDoubleValidator(double from, double to)
        {
            return str =>
            {
                var val = str.TryParseToDoubleInvariant();

                return val >= from && val <= to;
            };
        }

        public static Func<string, bool> GetInt32Validator(int from, int to)
        {
            return str =>
            {
                var val = str.TryParseToInt32Invariant();

                return val >= from && val <= to;
            };
        }
    }
}
