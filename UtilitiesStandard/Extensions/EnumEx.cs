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
    public static class EnumEx
    {
        /// <summary>
        /// Gets an attribute on an enum field value
        /// </summary>
        /// <typeparam name="T">The type of the attribute you want to retrieve</typeparam>
        /// <param name="enumVal">The enum value</param>
        /// <returns>The attribute of type T that exists on the enum value</returns>
        /// <example>string desc = myEnumVariable.GetAttributeOfType<DescriptionAttribute>().Description;</example>
        public static T GetAttribute<T>(this Enum enumVal)
            where T : Attribute
        {
            var type = enumVal.GetType();
            var memInfo = type.GetMember(enumVal.ToString()).Single();
            var attributes = memInfo.GetCustomAttributes(typeof(T), false);
            return (attributes.Length > 0)
                ? (T)attributes.Single()
                : null;
        }

        public static bool HasAttribute<T>(this Enum enumVal)
            where T : Attribute
        {
            return enumVal.GetAttribute<T>() != null;
        }

        public static string ToString<T>(this T value, params (T value, string asText)[] dictonary)
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be Enum");
            }

            foreach (var kvp in dictonary)
            {
                if (kvp.value.Equals(value))
                {
                    return kvp.asText;
                }
            }

            throw new NotSupportedException();
        }
        public static string ToString<T>(this T value, Func<T, string> defaultConvertor, params (T value, string asText)[] dictonary)
        {
            if (!typeof(T).IsEnum)
            {
                throw new ArgumentException("T must be Enum");
            }

            foreach (var kvp in dictonary)
            {
                if (kvp.value.Equals(value))
                {
                    return kvp.asText;
                }
            }

            return defaultConvertor(value);
        }
    }
}
