using System;

namespace Utilities.Types
{
    public class PathFormat
    {
        public static readonly PathFormat DEFAULT = new PathFormat("\\", ":\\");

        public string PartsSeparator { get; }
        public string RootMarker { get; }

        public PathFormat(string partsSeparator) : this(partsSeparator, null)
        {

        }
        public PathFormat(string partsSeparator, string rootMarker)
        {
            PartsSeparator = partsSeparator ?? throw new ArgumentNullException(nameof(partsSeparator));
            RootMarker = rootMarker;
        }
    }
}
