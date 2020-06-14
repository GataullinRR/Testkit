using System;
using System.Collections.Generic;
using Utilities;
using Utilities.Extensions;
using Utilities.Types;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Dynamic;

namespace Utilities.Extensions
{
    public static class DynamicEx
    {
        public static ExpandoObject MemberwiseClone(this ExpandoObject obj)
        {
            dynamic copied = new ExpandoObject();
            foreach (var kvp in obj)
            {
                copied[kvp.Key] = kvp.Value;
            }

            return copied;
        }
    }
}
