using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.Types
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class AutoMapToAttribute : Attribute
    {
        public AutoMapToAttribute(Type to)
        {
            To = to;
        }

        public Type To { get; }
    }
}
