using System;

namespace Utilities.Types
{
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class IncludeAttribute : Attribute
    {
        public string[] Groups { get; }

        public IncludeAttribute(params string[] groups)
        {
            Groups = groups ?? throw new ArgumentNullException(nameof(groups));
        }
    }
}
