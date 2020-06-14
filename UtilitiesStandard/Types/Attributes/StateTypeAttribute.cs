using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.Types
{
    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = false)]
    public class StateTypeAttribute : Attribute
    {
        public bool Success { get; }

        public StateTypeAttribute(bool success)
        {
            Success = success;
        }
    }
}
