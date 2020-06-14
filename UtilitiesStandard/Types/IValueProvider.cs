namespace Utilities.Types
{
    public interface IValueProvider<T>
    {
        T Value { get; }
    }
}