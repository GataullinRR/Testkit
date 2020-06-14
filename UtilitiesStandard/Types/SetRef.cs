using System;

namespace Utilities.Types
{
    public class SetRef<T>
    {
        public static implicit operator SetRef<T>(Action<T> setAction)
        {
            return new SetRef<T>(setAction);
        }
        public static implicit operator SetRef<T>(Func<T, T> setFunc)
        {
            return new SetRef<T>(v => setFunc(v));
        }

        readonly Action<T> _setter;

        public T Value
        {
            set { _setter(value); }
        }

        public SetRef(Action<T> setter)
        {
            _setter = setter ?? throw new ArgumentNullException();
        }
    }
}
