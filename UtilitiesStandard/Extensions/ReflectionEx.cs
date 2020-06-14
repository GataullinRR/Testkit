using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.Extensions;
using Utilities.Types;

namespace Utilities.Extensions
{
    public static class ReflectionEx
    {
        public static bool IsStatic(this Type t)
        {
            return t.IsAbstract && t.IsSealed;
        }

        public static object GetDefaultValue(this Type t)
        {
            return t.IsValueType ? Activator.CreateInstance(t) : null;
        }
    }
}
