namespace Utilities.Types
{
    public class SmartInt
    {
        public static implicit operator int(SmartInt si)
        {
            return si.Value;
        }

        int _Value;

        public int DValue { get; private set; }
        public int Value
        {
            get => _Value;
            set
            {
                DValue = value - Value;
                _Value = value;
            }
        }
        public int PreviousValue => Value - DValue;

        public SmartInt Add(int value)
        {
            Value += value;

            return this;
        }
    }
}
