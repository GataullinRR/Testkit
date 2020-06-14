namespace Utilities.Types
{
    public class ValueProvider<T> : IValueProvider<T>
    {
        public T Value { get; }

        public ValueProvider(T value)
        {
            Value = value;
        }
    }
}
