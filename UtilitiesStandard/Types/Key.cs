using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.Types
{
    public readonly struct Key : IEquatable<Key>
    {
        public static implicit operator Key(string key)
        {
            return new Key(key);
        }
        public static bool operator ==(Key left, Key right)
        {
            return left.Equals(right);
        }
        public static bool operator !=(Key left, Key right)
        {
            return !(left == right);
        }

        readonly string _key;

        public Key(string key)
        {
            _key = key;
        }

        public bool Equals(Key other)
        {
            return _key.Equals(other._key);
        }

        public override bool Equals(object obj)
        {
            if (obj is Key key)
            {
                return key.Equals(this);
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return _key.GetHashCode();
        }

        public override string ToString()
        {
            return _key;
        }
    }
}
