using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities;
using Utilities.Extensions;

namespace Utilities.Types
{
    public class CustomEventArgs<T> : EventArgs
    {
        public static implicit operator CustomEventArgs<T>(T value)
        {
            return new CustomEventArgs<T>(value);
        }
        public static implicit operator T(CustomEventArgs<T> value)
        {
            return value.Value;
        }

        readonly T _value;
        public T Value { get => _value; }

        public CustomEventArgs(T value)
        {
            _value = value;
        }
    }
}
