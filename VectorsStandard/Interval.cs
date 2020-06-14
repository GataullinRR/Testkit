using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities;
using Utilities.Extensions;
using Utilities.Types;
using static System.Math;

namespace Vectors
{
    [Serializable]
    public readonly struct Interval : IComparable<Interval>, IFormattable
    {
        public static readonly Interval Zero = new Interval(0);
        public static readonly Interval Max = new Interval(double.MinValue, double.MaxValue);

        public readonly double From;
        public readonly double To;

        #region ##### OPERATORS #####

        public static Interval operator %(Interval l, int r)
        {
            return new Interval(l.From % r, l.To % r);
        }
        public static Interval operator %(Interval l, float r)
        {
            return new Interval(l.From % r, l.To % r);
        }
        public static Interval operator %(Interval l, double r)
        {
            return new Interval(l.From % r, l.To % r);
        }
        public static Interval operator -(Interval l, Interval r)
        {
            return new Interval(l.From - r.From, l.To - r.To);
        }
        public static Interval operator -(Interval l, float r)
        {
            return new Interval(l.From - r, l.To - r);
        }
        public static Interval operator -(Interval l, int r)
        {
            return new Interval(l.From - r, l.To - r);
        }
        public static Interval operator -(Interval l, double r)
        {
            return new Interval(l.From - r, l.To - r);
        }
        public static Interval operator +(Interval l, Interval r)
        {
            return new Interval(l.From + r.From, l.To + r.To);
        }
        public static Interval operator +(Interval l, float r)
        {
            return new Interval(l.From + r, l.To + r);
        }
        public static Interval operator +(Interval l, int r)
        {
            return new Interval(l.From + r, l.To + r);
        }
        public static Interval operator +(Interval l, double r)
        {
            return new Interval(l.From + r, l.To + r);
        }
        public static Interval operator /(Interval l, Interval r)
        {
            return new Interval(l.From / r.From, l.To / r.To);
        }
        public static Interval operator /(Interval l, float r)
        {
            return new Interval(l.From / r, l.To / r);
        }
        public static Interval operator /(Interval l, int r)
        {
            return new Interval(l.From / r, l.To / r);
        }
        public static Interval operator /(Interval l, double r)
        {
            return new Interval(l.From / r, l.To / r);
        }
        public static Interval operator *(Interval l, Interval r)
        {
            return new Interval(l.From * r.From, l.To * r.To);
        }
        public static Interval operator *(Interval l, float r)
        {
            return new Interval(l.From * r, l.To * r);
        }
        public static Interval operator *(Interval l, int r)
        {
            return new Interval(l.From * r, l.To * r);
        }
        public static Interval operator *(Interval l, double r)
        {
            return new Interval(l.From * r, l.To * r);
        }

        public static bool operator >(Interval l, Interval r)
        {
            return l.AbsLen > r.AbsLen;
        }
        public static bool operator <(Interval l, Interval r)
        {
            return l.AbsLen < r.AbsLen;
        }
        public static bool operator ==(Interval l, Interval r)
        {
            return l.From == r.From && l.To == r.To;
        }
        public static bool operator !=(Interval l, Interval r)
        {
            return l == r == false;
        }

        #endregion

        #region ##### CTORS #####

        public Interval(V2 vector)
        {
            From = vector.X;
            To = vector.Y;
        }

        public Interval(float both)
        {
            From = both;
            To = both;
        }
        public Interval(int both)
        {
            From = both;
            To = both;
        }
        public Interval(double both)
        {
            From = both;
            To = both;
        }

        public Interval(float from, float to)
        {
            From = from;
            To = to;
        }
        public Interval(int from, int to)
        {
            From = from;
            To = to;
        }
        public Interval(double from, double to)
        {
            From = from;
            To = to;
        }

        #endregion

        public bool IsZero => From == 0 && To == 0;
        public bool IsNegative => From < 0 && To < 0;
        public bool IsNegativeWithZero => From <= 0 && To <= 0;
        public bool IsPositive => From > 0 && To > 0;
        public bool IsPositiveWithZero => From >= 0 && To >= 0;

        //public bool IsCrossingZero => From.Sign() != To.Sign() || From == 0 || To == 0;

        public bool IsPoint => From == To;

        public int IntFrom => From.ToInt32();
        public int IntTo => To.ToInt32();
        public V2 AsV2 => new V2(From, To);
        public IntInterval AsIntInterval => new IntInterval(From, To);
        public Interval Round => new Interval(From.Round(), To.Round());
        public Interval SfiftToFromInteger => this + (From.Round() - From);
        public Interval Swap => new Interval(To, From);
        /// <summary>
        /// Swaps values if <see cref="From"/> > <see cref="To"/> 
        /// </summary>
        public Interval Fix => From > To ? Swap : this;
        public Interval NegativeToZero => new Interval(From.NegativeToZero(), To.NegativeToZero());

        public double Middle => From + Len / 2;
        public int IntMiddle => Middle.Round();

        public double Len => To - From;
        public double AbsLen => Abs(Len);
        public int IntLen => Len.ToInt32();
        public int IntAbsLen => AbsLen.ToInt32();

        public double RandomPoint => GetRandomPoint();

        public double GetRandomPoint()
        {
            return GetRandomPoint(Global.Random);
        }
        public double GetRandomPoint(Random random)
        {
            return random.NextDouble(From, To);
        }

        public Interval SetFrom(double from)
        {
            return new Interval(from, To);
        }
        public Interval SetTo(double to)
        {
            return new Interval(From, to);
        }

        public Interval Shift(double distance)
        {
            return this + distance;
        }
        public Interval Mul(double value)
        {
            return this * value;
        }
        public Interval Div(double value)
        {
            return this / value;
        }

        public IEnumerable<Interval> DivideEvenly(int numOfSubintervals)
        {
            var dLen = Len / numOfSubintervals;
            var previous = ByLen(From, dLen);
            yield return previous;
            for (int i = 1; i < numOfSubintervals; i++)
            {
                previous = ByLen(previous.To, dLen);
                yield return previous;
            }
        }

        /// <summary>
        /// Expands <see cref="Interval"/> to contain <paramref name="value"/>
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Interval ExpandToContain(double value)
        {
            if (To < value)
            {
                return SetTo(value);
            }
            else if (value < From)
            {
                return SetFrom(value);
            }
            else if (To < value && value < From)
            {
                return new Interval(value);
            }
            else
            {
                return this;
            }
        }

        public double OverlapLen(Interval otherInterval)
        {
            var maxOverlap = Min(AbsLen, otherInterval.AbsLen);
            var dLen = (otherInterval.AbsLen - AbsLen).Abs();
            var offset = otherInterval.From - From;
            if (Contains(otherInterval) || otherInterval.Contains(this))
            {
                return maxOverlap;
            }
            else if (offset >= 0)
            {
                return (maxOverlap - offset).NegativeToZero();
            }
            else
            {
                return (maxOverlap + offset).NegativeToZero();
            }
        }

        public bool Touches(Interval interval, bool strict = false)
        {
            return interval.Contains(From, strict) || interval.Contains(To, strict) ||
                Contains(interval.From, strict) || Contains(interval.To, strict);
        }

        public bool Contains(Interval interval, bool strict = false)
        {
            return Contains(interval.From, strict) && Contains(interval.To, strict);
        }
        public bool Contains(double point, bool strict = false)
        {
            if (!strict)
            {
                return Len > 0 
                    ? (point >= From && point <= To)
                    : (point >= To && point <= From);
            }
            else
            {
                return Len > 0
                    ? (point > From && point < To)
                    : (point > To && point < From);
            }
        }

        public static Interval ByPos(double from, double to)
        {
            return new Interval(from, to);
        }
        public static Interval ByLen(double from, double length)
        {
            return new Interval(from, from + length);
        }
        public static Interval ByCenter(double center, double offsetFromCenter)
        {
            return new Interval(center - offsetFromCenter, center + offsetFromCenter);
        }

        public static Interval Parse(string str)
        {
            var from_to = str.Remove(" ").Split(":");
            if (from_to.Length != 2)
            {
                throw new FormatException();
            }
            var from = parse(from_to[0]);
            var to = parse(from_to[1]);
            return new Interval(from, to);

            /////////////////////////

            double parse(string s)
            {
                return s.SkipWhile(c => !isNumberChar(c))
                        .TakeWhile(isNumberChar)
                        .Aggregate()
                        .ParseToDoubleInvariant();

                bool isNumberChar(char c) => char.IsDigit(c) || c == '.' || c == ',';
            }
        }

        #region ##### OBJECT OVERRIDE #####

        public override bool Equals(object obj)
        {
            if (obj is Interval interval)
            {
                return Equals(interval);
            }
            else
            {
                return false;
            }
        }
        public bool Equals(Interval i)
        {
            return this == i;
        }

        public override int GetHashCode()
        {
            // https://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode
            return new { From, To }.GetHashCode();
        }

        public override string ToString()
        {
            return ToString(null, null);
        }

        #endregion

        public int CompareTo(Interval other)
        {
            int ret;

            if (Len < other.Len)
                ret = -1;
            else if (Len > other.Len)
                ret = 1;
            else if (Len == other.Len)
                ret = 0;
            else
                throw new InvalidOperationException();

            return ret;
        }

        /// <summary>
        /// null = $"{From} : {To}"
        /// "L" = $"{From} : {To} Len = {Len}"
        /// </summary>
        /// <param name="format"></param>
        /// <param name="formatProvider"></param>
        /// <returns></returns>
        public string ToString(string format, IFormatProvider formatProvider)
        {
            if (formatProvider != null)
            {
                throw new ArgumentUniformException(ArgumentError.NOT_SUPPORTED_BY_CURRENT_IMPL);
            }
            switch (format)
            {
                case null:
                    return $"{From.ToStringInvariant()} : {To.ToStringInvariant()}";
                case "L":
                    return $"{From.ToStringInvariant()} : {To.ToStringInvariant()} Len = {Len.ToStringInvariant()}";
                default:
                    throw new ArgumentUniformException(ArgumentError.OUT_OF_RANGE);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value">{From} : {To}</param>
        /// <param name="interval"></param>
        /// <returns></returns>
        public static bool TryParse(string value, out Interval interval)
        {
            try
            {
                var fromTo = value.Remove(" ").Split(":");
                interval = new Interval(fromTo[0].ParseToDoubleInvariant(), fromTo[1].ParseToDoubleInvariant());

                return true;
            }
            catch
            {
                interval = default;

                return false;
            }
        }
    }
}