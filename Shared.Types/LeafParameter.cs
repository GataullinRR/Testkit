using System;
using System.Collections.Generic;

namespace SharedT.Types
{
    public class LeafParameter : ParameterBase, IEquatable<LeafParameter?>
    {
        public string? Value { get; set; }

        public override bool Equals(object? obj)
        {
            return Equals(obj as LeafParameter);
        }

        public bool Equals(LeafParameter? other)
        {
            return other != null &&
                   GetFullPath() == other.GetFullPath() &&
                   Name == other.Name &&
                   EqualityComparer<ParameterBase?>.Default.Equals(Parrent, other.Parrent) &&
                   Value == other.Value;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, Parrent, Value);
        }

        public static bool operator ==(LeafParameter? left, LeafParameter? right)
        {
            return EqualityComparer<LeafParameter>.Default.Equals(left, right);
        }

        public static bool operator !=(LeafParameter? left, LeafParameter? right)
        {
            return !(left == right);
        }
    }
}
