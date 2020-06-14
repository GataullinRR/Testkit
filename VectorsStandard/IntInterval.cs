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
    public readonly struct IntInterval : IComparable<IntInterval>, IFormattable
    {
        public static readonly IntInterval Zero = new IntInterval(0);

        public readonly int From;
        public readonly int To;

        #region ##### OPERATORS #####

        public static implicit operator Interval(IntInterval interval)
        {
            return new Interval(interval.From, interval.To);
        }
        public static explicit operator IntInterval(Interval interval)
        {
            return new IntInterval(interval.From.Round(), interval.To.Round());
        }

        public static IntInterval operator %(IntInterval l, int r)
        {
            return new IntInterval(l.From % r, l.To % r);
        }
        public static IntInterval operator %(IntInterval l, float r)
        {
            return new IntInterval(l.From % r, l.To % r);
        }
        public static IntInterval operator %(IntInterval l, double r)
        {
            return new IntInterval(l.From % r, l.To % r);
        }
        public static IntInterval operator -(IntInterval l, IntInterval r)
        {
            return new IntInterval(l.From - r.From, l.To - r.To);
        }
        public static IntInterval operator -(IntInterval l, float r)
        {
            return new IntInterval(l.From - r, l.To - r);
        }
        public static IntInterval operator -(IntInterval l, int r)
        {
            return new IntInterval(l.From - r, l.To - r);
        }
        public static IntInterval operator -(IntInterval l, double r)
        {
            return new IntInterval(l.From - r, l.To - r);
        }
        public static IntInterval operator +(IntInterval l, IntInterval r)
        {
            return new IntInterval(l.From + r.From, l.To + r.To);
        }
        public static IntInterval operator +(IntInterval l, float r)
        {
            return new IntInterval(l.From + r, l.To + r);
        }
        public static IntInterval operator +(IntInterval l, int r)
        {
            return new IntInterval(l.From + r, l.To + r);
        }
        public static IntInterval operator +(IntInterval l, double r)
        {
            return new IntInterval(l.From + r, l.To + r);
        }
        public static IntInterval operator /(IntInterval l, IntInterval r)
        {
            return new IntInterval(l.From / r.From, l.To / r.To);
        }
        public static IntInterval operator /(IntInterval l, float r)
        {
            return new IntInterval(l.From / r, l.To / r);
        }
        public static IntInterval operator /(IntInterval l, int r)
        {
            return new IntInterval(l.From / r, l.To / r);
        }
        public static IntInterval operator /(IntInterval l, double r)
        {
            return new IntInterval(l.From / r, l.To / r);
        }
        public static IntInterval operator *(IntInterval l, IntInterval r)
        {
            return new IntInterval(l.From * r.From, l.To * r.To);
        }
        public static IntInterval operator *(IntInterval l, float r)
        {
            return new IntInterval(l.From * r, l.To * r);
        }
        public static IntInterval operator *(IntInterval l, int r)
        {
            return new IntInterval(l.From * r, l.To * r);
        }
        public static IntInterval operator *(IntInterval l, double r)
        {
            return new IntInterval(l.From * r, l.To * r);
        }

        public static bool operator >(IntInterval l, IntInterval r)
        {
            return l.AbsLen > r.AbsLen;
        }
        public static bool operator <(IntInterval l, IntInterval r)
        {
            return l.AbsLen < r.AbsLen;
        }
        public static bool operator ==(IntInterval l, IntInterval r)
        {
            return l.From == r.From && l.To == r.To;
        }
        public static bool operator !=(IntInterval l, IntInterval r)
        {
            return l == r == false;
        }

        #endregion

        #region ##### CTORS #####

        public IntInterval(V2 vector)
        {
            From = vector.IntX;
            To = vector.IntY;
        }

        public IntInterval(float both)
        {
            From = both.ToInt32();
            To = both.ToInt32();
        }
        public IntInterval(int both)
        {
            From = both;
            To = both;
        }
        public IntInterval(double both)
        {
            From = both.ToInt32();
            To = both.ToInt32();
        }

        public IntInterval(float from, float to)
        {
            From = from.ToInt32();
            To = to.ToInt32();
        }
        public IntInterval(int from, int to)
        {
            From = from;
            To = to;
        }
        public IntInterval(double from, double to)
        {
            From = from.ToInt32();
            To = to.ToInt32();
        }

        #endregion

        public bool IsZero => From == 0 && To == 0;
        public bool IsNegative => From < 0 && To < 0;
        public bool IsNegativeWithZero => From <= 0 && To <= 0;
        public bool IsPositive => From > 0 && To > 0;
        public bool IsPositiveWithZero => From >= 0 && To >= 0;
        public bool IsPoint => From == To;

        public V2 AsV2 => new V2(From, To);
        public Interval AsInterval => new Interval(From, To);

        public int Len => To - From;
        public int AbsLen => Abs(Len);
        public int Middle => (From + Len / 2D).Round();

        public IntInterval SetFrom(int from)
        {
            return new IntInterval(from, To);
        }
        public IntInterval SetTo(int to)
        {
            return new IntInterval(From, to);
        }

        public IntInterval Add(int value)
        {
            return this + value;
        }
        public IntInterval AddFrom(int fromValue)
        {
            return new IntInterval(From + fromValue, To);
        }
        public IntInterval AddTo(int toValue)
        {
            return new IntInterval(From, To + toValue);
        }
        public IntInterval Sub(int value)
        {
            return this - value;
        }
        public IntInterval SubFrom(int fromValue)
        {
            return new IntInterval(From - fromValue, To);
        }
        public IntInterval SubTo(int toValue)
        {
            return new IntInterval(From, To - toValue);
        }
        public IntInterval Add(int fromValue, int toValue)
        {
            return this + new IntInterval(fromValue, toValue);
        }
        public IntInterval Sub(int fromValue, int toValue)
        {
            return this - new IntInterval(fromValue, toValue);
        }

        public int OverlapLen(IntInterval otherInterval)
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

        public bool Touches(IntInterval interval, bool strict = false)
        {
            return interval.Contains(From, strict) 
                || interval.Contains(To, strict) 
                || Contains(interval.From, strict) 
                || Contains(interval.To, strict);
        }

        public bool Contains(IntInterval intInterval, bool strict = false)
        {
            return Contains(intInterval.From, strict) 
                && Contains(intInterval.To, strict);
        }
        public bool Contains(double point, bool strict = false)
        {
            if (!strict)
            {
                return point >= From && point <= To;
            }
            else
            {
                return point > From && point < To;
            }
        }

        #region ##### STATIC FACTORIES #####

        public static IntInterval ByPos(int from, int to)
        {
            return new IntInterval(from, to);
        }
        public static IntInterval ByLen(int from, int length)
        {
            return new IntInterval(from, from + length);
        }
        public static IntInterval ByLen(double from, double length)
        {
            return new IntInterval(from, from + length);
        }
        public static IntInterval ByLenAndEnd(int end, int length)
        {
            return new IntInterval(end - length, end);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="center"></param>
        /// <param name="offsetFromCenter">For both directions</param>
        /// <returns></returns>
        public static IntInterval ByCenter(int center, int offsetFromCenter)
        {
            return new IntInterval(center - offsetFromCenter, center + offsetFromCenter);
        }
        public static IntInterval ByCenter(double center, double offsetFromCenter)
        {
            return new IntInterval(center - offsetFromCenter, center + offsetFromCenter);
        }

        public static IntInterval Parse(string interval)
        {
            var fromTo = interval.Split(":");

            return new IntInterval(fromTo[0].ParseToInt32Invariant(), fromTo[1].ParseToInt32Invariant());
        }
        public static IEnumerable<IntInterval> ParseArray(string intervals)
        {
            return intervals.Split(" ").Select(Parse);
        }

        #endregion

        #region ##### OBJECT OVERRIDE #####

        public override bool Equals(object obj)
        {
            if (obj is IntInterval IntInterval)
            {
                return Equals(IntInterval);
            }
            else
            {
                return false;
            }
        }
        public bool Equals(IntInterval i)
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
            return ToString("L", null);
        }

        #endregion

        public int CompareTo(IntInterval other)
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
        /// "L" = $"{From} : {To} / {Len}"
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
                    return $"{From} : {To}";
                case "L":
                    return $"{From} : {To} / {Len}";
                default:
                    throw new ArgumentUniformException(ArgumentError.OUT_OF_RANGE);
            }
        }
    }
}