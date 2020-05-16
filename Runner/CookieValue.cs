using System;

namespace Runner
{
    public class CookieValue
    {
        public static implicit operator CookieValue(string value)
        {
            return new CookieValue(value);
        }

        public static implicit operator string?(CookieValue value)
        {
            return value?.Value;
        }

        public string Value { get; }
        public DateTime ExpireAt { get; }

        public CookieValue(string value) : this(value, TimeSpan.FromDays(365))
        {

        }
        public CookieValue(string value, TimeSpan duration)
        {
            Value = value;
            ExpireAt = DateTime.UtcNow.Add(duration);
        }
    }
}
