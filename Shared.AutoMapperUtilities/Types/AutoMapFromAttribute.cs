using System;

namespace Utilities.Types
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
    public class AutoMapFromAttribute : Attribute
    {
        public AutoMapFromAttribute(Type from)
        {
            From = from;
        }

        public Type From { get; }
    }
}
