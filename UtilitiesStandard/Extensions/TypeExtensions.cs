using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.Extensions
{
    public static class TypeExtensions
    {
        public static string GetTypeClassName(this Type t)
        {
            return t.Name;
        }
    }
}
