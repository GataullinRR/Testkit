using System;
using System.Collections.Generic;
using System.Text;
using Utilities.Types;

namespace Utilities.Extensions
{
    public static class AttributesEx
    {
        public static bool IsSuccessState(this Enum enumObject)
        {
            return enumObject.GetAttribute<StateTypeAttribute>().Success; 
        }
    }
}
