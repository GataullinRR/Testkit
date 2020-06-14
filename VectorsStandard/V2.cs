//#define INC_MATRIX_OP
//#define INC_XNA_VECTORS_OP
//#define INC_V3_OP
//#define INC_V4_OP
//#define INC_MYMATH_OP
//#define INC_EDGE_OP
//#define INC_SYSTEM_DRAWING_OP
//#define INC_UNITY3D_OP

using System;
using System.Collections.Generic;
using System.Text;
#if INC_SYSTEM_DRAWING_OP
using System.Drawing;
#endif
using System.Linq;
using Utilities;
using Utilities.Extensions;
#if INC_UNITY3D_OP
using UnityEngine;
#endif

namespace Vectors
{
    [Serializable]
    public readonly partial struct V2
    {
        public enum RelativePos : byte
        {
            LEFT,
            RIGHT,
            BEYOND,  // Cпереди
            BEHIND,  // Позади
            BETWEEN,
            ORIGIN,
            DESTINATION
        }

        public const double PI = Math.PI;
        public const double PIPI = 2 * Math.PI;
        public const double RAD_TO_DEG = 180 / PI;
        public const double DEG_TO_RAD = PI / 180;

        public static readonly V2 PositiveInfinity = new V2(double.PositiveInfinity, double.PositiveInfinity);
        public static readonly V2 NegativeInfinity = new V2(double.NegativeInfinity, double.NegativeInfinity);
        public static readonly V2 MaxValue = new V2(double.MaxValue, double.MaxValue);
        public static readonly V2 MinValue = new V2(double.MinValue, double.MinValue);
        public static readonly V2 NaN = new V2(double.NaN, double.NaN);
        public static readonly V2 Zero = new V2(0);
        public static readonly V2 One = new V2(1);
        public static readonly V2 Half = new V2(0.5);

        public readonly double X;
        public readonly double Y;

        #region ##### OPERATORS #####

        public static V2 operator %(V2 l, int r)
        {
            return new V2(l.X % r, l.Y % r);
        }
        public static V2 operator %(V2 l, float r)
        {
            return new V2(l.X % r, l.Y % r);
        }
        public static V2 operator %(V2 l, double r)
        {
            return new V2(l.X % r, l.Y % r);
        }
        public static V2 operator -(V2 v1, V2 v2)
        {
            return new V2(v1.X - v2.X, v1.Y - v2.Y);
        }
        public static V2 operator -(V2 l, float r)
        {
            return new V2(l.X - r, l.Y - r);
        }
        public static V2 operator -(V2 l, int r)
        {
            return new V2(l.X - r, l.Y - r);
        }
        public static V2 operator -(V2 l, double r)
        {
            return new V2(l.X - r, l.Y - r);
        }
        public static V2 operator +(V2 v1, V2 v2)
        {
            return new V2(v1.X + v2.X, v1.Y + v2.Y);
        }
        public static V2 operator +(V2 l, float r)
        {
            return new V2(l.X + r, l.Y + r);
        }
        public static V2 operator +(V2 l, int r)
        {
            return new V2(l.X + r, l.Y + r);
        }
        public static V2 operator +(V2 l, double r)
        {
            return new V2(l.X + r, l.Y + r);
        }
        public static V2 operator /(V2 v1, V2 v2)
        {
            return new V2(v1.X / v2.X, v1.Y / v2.Y);
        }
        public static V2 operator /(V2 l, float r)
        {
            return new V2(l.X / r, l.Y / r);
        }
        public static V2 operator /(V2 l, int r)
        {
            return new V2(l.X / r, l.Y / r);
        }
        public static V2 operator /(V2 l, double r)
        {
            return new V2(l.X / r, l.Y / r);
        }
        public static V2 operator *(V2 v1, V2 v2)
        {
            return new V2(v1.X * v2.X, v1.Y * v2.Y);
        }
        public static V2 operator *(V2 l, float r)
        {
            return new V2(l.X * r, l.Y * r);
        }
        public static V2 operator *(float l, V2 r)
        {
            return new V2(r.X * l, r.Y * l);
        }
        public static V2 operator *(V2 l, int r)
        {
            return new V2(l.X * r, l.Y * r);
        }
        public static V2 operator *(int l, V2 r)
        {
            return new V2(r.X * l, r.Y * l);
        }
        public static V2 operator *(V2 l, double r)
        {
            return new V2(l.X * r, l.Y * r);
        }
        public static bool operator >(V2 l, V2 r)
        {
            return l.Len > r.Len;
        }
        public static bool operator <(V2 l, V2 r)
        {
            return l.Len < r.Len;
        }
        public static bool operator ==(V2 l, V2 r)
        {
            return l.X == r.X && l.Y == r.Y;
        }
        public static bool operator !=(V2 l, V2 r)
        {
            return !(l.X == r.X && l.Y == r.Y);
        }

        #endregion

        #region ##### CTORS #####

        public V2(float f)
        {
            X = f;
            Y = f;
        }
        public V2(int i)
        {
            X = i;
            Y = i;
        }
        public V2(double d)
        {
            X = d;
            Y = d;
        }

        public V2(float x, float y)
        {
            X = x;
            Y = y;
        }
        public V2(int x, int y)
        {
            X = x;
            Y = y;
        }
        public V2(double x, double y)
        {
            X = x;
            Y = y;
        }

        #endregion

        #region ##### METHODS #####

        #region ##### SET #####

        public V2 SetX(int value)
        {
            return new V2(value, Y);
        }
        public V2 SetX(float value)
        {
            return new V2(value, Y);
        }
        public V2 SetX(double value)
        {
            return new V2(value, Y);
        }
        public V2 SetX(string value)
        {
            return new V2(value.ParseToDoubleInvariant(), Y);
        }

        public V2 SetY(int value)
        {
            return new V2(X, value);
        }
        public V2 SetY(float value)
        {
            return new V2(X, value);
        }
        public V2 SetY(double value)
        {
            return new V2(X, value);
        }
        public V2 SetY(string value)
        {
            return new V2(X, value.ParseToDoubleInvariant());
        }

        #endregion

        public double IntervalLen => Math.Abs(X - Y); 
        public double Len => Math.Sqrt(SqLen);
        public double SqLen => X * X + Y * Y;
        public double MaxComponent => X >= Y ? X : Y;
        public double MinComponent => X <= Y ? X : Y;
        public int IntX => (int)Math.Round(X);
        public int IntY => (int)Math.Round(Y);
        public bool IsXOrYNaN => double.IsNaN(X) || double.IsNaN(Y);
        public bool IsXAndYNaN => double.IsNaN(X) && double.IsNaN(Y);
        public bool IsXOrYNegative => X < 0 || Y < 0;
        public bool IsXAndYNegative => X < 0 && Y < 0;
        public bool IsXOrYPositive => X > 0 || Y > 0;
        public bool IsXAndYPositive => X > 0 && Y > 0;
        public bool IsZero => X == 0 && Y == 0;

        public V2 Ceiling => new V2(Math.Ceiling(X), Math.Ceiling(Y));
        public V2 Floor => new V2(Math.Floor(X), Math.Floor(Y));
        public V2 SwapXY => new V2(Y, X);
        public V2 Norm => new V2(X, Y) / Math.Sqrt(X * X + Y * Y);
        public V2 InvY => new V2(X, -Y);
        public V2 InvX => new V2(-X, Y);
        public V2 Inv => new V2(-X, -Y);
        public V2 Abs => new V2(Math.Abs(X), Math.Abs(Y));
        public V2 LPerp => new V2(-Y, X);
        public V2 RPerp => LPerp.Inv;

        public RelativePos Classify(V2 p0, V2 p1)
        {
            V2 @this = new V2(X, Y);
            V2 a = p1 - p0;
            V2 b = @this - p0;

            double sa = a.X * b.Y - b.X * a.Y;
            if (sa > 0)
                return RelativePos.LEFT;
            else if (sa < 0)
                return RelativePos.RIGHT;
            else if ((a.X * b.X < 0) || (a.Y * b.Y < 0))
                return RelativePos.BEHIND;
            else if (a.Len < b.Len)
                return RelativePos.BEYOND;
            else if (p0 == @this)
                return RelativePos.ORIGIN;
            else if (p1 == @this)
                return RelativePos.DESTINATION;
            else
                return RelativePos.BETWEEN;
        }
        public double Dot(V2 secondV2)
        {
            return X * secondV2.X + Y * secondV2.Y;
        }
        public double Cross(V2 second)
        {
            return X * second.Y - Y * second.X;
        }
        public V2 Round(int digits)
        {
            return new V2(Math.Round(X, digits), Math.Round(Y, digits));
        }
        public V2 Pow(double p)
        {
            return new V2(Math.Pow(X, p), Math.Pow(Y, p));
        }
        public V2 Add(double value)
        {
            return this + value;
        }
        public V2 AddX(double xValue)
        {
            return new V2(X + xValue, Y);
        }
        public V2 AddY(double yValue)
        {
            return new V2(X, Y + yValue);
        }
        public V2 Sub(double value)
        {
            return this - value;
        }
        public V2 SubX(double xValue)
        {
            return new V2(X - xValue, Y);
        }
        public V2 SubY(double yValue)
        {
            return new V2(X, Y - yValue);
        }
        public V2 Mul(double k)
        {
            return this * k;
        }
        public V2 Div(double k)
        {
            return this / k;
        }
        public V2 Add(double x, double y)
        {
            return this + new V2(x, y);
        }
        public V2 Sub(double x, double y)
        {
            return this - new V2(x, y);
        }
        public V2 Add(V2 anotherVector)
        {
            return this + anotherVector;
        }
        public V2 Sub(V2 anotherVector)
        {
            return this - anotherVector;
        }
        public V2 Mul(double xk, double yk)
        {
            return this * new V2(xk, yk);
        }
        public V2 Mul(V2 k)
        {
            return this * k;
        }
        public V2 Div(double xk, double yk)
        {
            return this / new V2(xk, yk);
        }
        public V2 Div(V2 k)
        {
            return this / k;
        }

        public V2 ReflectY(double ySize)
        {
            return new V2(X, ySize - Y);
        }
        public V2 Rotate(V2 rotPoint, double angleDeg)
        {
            double angleRad = angleDeg * DEG_TO_RAD;
            var x = rotPoint.X + (X - rotPoint.X) * Math.Cos(angleRad) - (Y - rotPoint.Y) * Math.Sin(angleRad);
            var y = rotPoint.Y + (Y - rotPoint.Y) * Math.Cos(angleRad) + (X - rotPoint.X) * Math.Sin(angleRad);

            return new V2(x, y);
        }

        /// <summary>
        /// Составляющая вектора this сонаправленная с second
        /// </summary>
        /// <param name="second"></param>
        /// <returns></returns>
        public V2 Projection(V2 second)
        {
            return second.Norm * (this * Dot(second.Norm) / Len).Len;
        }
        public static V2 Max(V2 first, V2 second)
        {
            return first.Len > second.Len ? first : second;
        }
        public static V2 Min(V2 first, V2 second)
        {
            return first.Len < second.Len ? first : second;
        }
        public bool IntervalContains(double val, bool strict)
        {
            if (strict)
                return (val > X) && (val < Y);
            else
                return (val >= X) && (val <= Y);
        }

#if INC_V3_OP
        /// <summary>
        /// Векторное произведение текущего вектора на вектор second Z = 0
        /// </ summary >
        /// < param name="second"></param>
        /// <returns></returns>
        public V3 VectMull(V2 second)
        {
            float z = X * second.Y - Y * second.X;
            return new V3(0, 0, z);
        }
#endif

#if INC_XNA_VECTORS_OP
        public Vector2 SV2 { get { return new Vector2(X, Y); } }
#endif

#if INC_UNITY3D_OP
        public Vector2 SV2 => new Vector2(X.ToSingle(), Y.ToSingle());
        public Vector3 SV3 => new Vector3(X.ToSingle(), Y.ToSingle());
#endif

#if INC_MYMATH_OP
        public V2 Random
        {
            get { return MyMath.RandomV2(new V2(-X, -Y), new V2(X, Y)); }
        }
#endif

#if INC_EDGE_OP
        public RelPos Classify(Edge e)
        {
            return Classify(e.Org, e.Dest);
        }
#endif

        #region ### ANGLES ###

        /// <summary>
        /// Возвращает минимальный угол между векторами, направление обсчета не учитывается.
        /// </summary>
        /// <returns>[0; 180]</returns>
        public double AcosDeg(V2 secondV2)
        {
            return 180 * AcosRad(secondV2) / RAD_TO_DEG;
        }
        public double AcosRad(V2 secondV2)
        {
            return Math.Acos(Dot(secondV2) / (Len * secondV2.Len));
        }
        public double AngleCos(V2 secondV2)
        {
            return Dot(secondV2) / (Len * secondV2.Len);
        }
        public double AngleSin(V2 secondV2)
        {
            return Cross(secondV2) / (Len * secondV2.Len);
        }
        public double AngleTan(V2 secondV2)
        {
            return (X * secondV2.Y - Y * secondV2.X) / (X * secondV2.X + Y * secondV2.Y);
        }

        /// <summary>
        /// Возвращает минимальный по модулю угол между this и secondV2, с учетом направления обсчета.
        /// </summary>
        /// <returns>(-180; 180]</returns>
        public double AngleDegSigned(V2 secondV2)
        {
            return AngleRadSigned(secondV2) * RAD_TO_DEG;
        }
        /// <summary>
        /// Возвращает минимальный по модулю угол между this и secondV2, с учетом направления обсчета.
        /// </summary>
        /// <returns>(-PI; PI]</returns>
        public double AngleRadSigned(V2 secondV2)
        {
            return Math.Atan2(X * secondV2.Y - secondV2.X * Y, X * secondV2.X + Y * secondV2.Y);
        }
        /// <summary>
        /// Возвращает угол между this и secondV2 образовынный при движении против часовой стрелки
        /// </summary>
        /// <returns>[0; 360]</returns>
        public double AngleDegCounterclockwise(V2 secondV2)
        {
            return AngleRadCounterclockwise(secondV2) * RAD_TO_DEG;
        }
        /// <summary>
        /// Возвращает угол между this и secondV2 образовынный при движении против часовой стрелки
        /// </summary>
        /// <returns>[0; 2PI]</returns>
        public double AngleRadCounterclockwise(V2 secondV2)
        {
            double radianSigned = AngleRadSigned(secondV2);
            if (radianSigned < 0) radianSigned += PIPI;
            return radianSigned;
        }
        /// <summary>
        /// Возвращает угол между this и secondV2 образовынный при движении по часовой стрелке
        /// </summary>
        /// <returns>[0; 360]</returns>
        public double AngleDegClockwise(V2 secondV2)
        {
            return 360 - AngleDegCounterclockwise(secondV2);
        }

        /// <summary>
        /// Возвращает угол между this и secondV2 образовынный при движении по часовой стрелке
        /// </summary>
        /// <returns>[0; 360]</returns>
        public double AngleRadClockwise(V2 secondV2)
        {
            return PIPI - AngleRadCounterclockwise(secondV2);
        }

        #endregion

        public static V2 Parse(string s, string separator)
        {
            s = s.Replace(separator, " ");
            string[] xy = s.Split(' ');

            return new V2(double.Parse(xy[0]), double.Parse(xy[1]));
        }

        #endregion

        #region ##### OBJECT OVERRIDE #####

        public override bool Equals(object obj)
        {
            if (obj is V2)
            {
                return Equals((V2)obj);
            }
            else
            {
                return false;
            }
        }
        public bool Equals(V2 v2)
        {
            return v2.X == X && v2.Y == Y;
        }

        public override int GetHashCode()
        {
            // https://stackoverflow.com/questions/263400/what-is-the-best-algorithm-for-an-overridden-system-object-gethashcode
            return new { X, Y }.GetHashCode();
        }

        public override string ToString()
        {
            return ToString("F4");
        }
        public string ToString(string format)
        {
            return string.Format($"X: {X.ToString(format)} Y: {Y.ToString(format)}");
        }

        public static V2 Parse(string str)
        {
            var substrs = str.Split(' ', ':');
            double x = double.Parse(substrs[2]);
            double y = double.Parse(substrs[5]);
            return new V2(x, y);
        }

        #endregion
    }

    public partial struct V2
    {
        #region ##### METHODS #####

#if INC_V3_OP
        /// <summary>
        /// Векторное произведение текущего вектора на вектор second Z = 0
        /// </ summary >
        /// < param name="second"></param>
        /// <returns></returns>
        public V3 VectMull(V2 second)
        {
            float z = X * second.Y - Y * second.X;
            return new V3(0, 0, z);
        }
#endif

#if INC_XNA_VECTORS_OP
        public Vector2 SV2 { get { return new Vector2(X, Y); } }
#endif

#if INC_MYMATH_OP
        public V2 Random
        {
            get { return MyMath.RandomV2(new V2(-X, -Y), new V2(X, Y)); }
        }
#endif

#if INC_EDGE_OP
        public RelPos Classify(Edge e)
        {
            return Classify(e.Org, e.Dest);
        }
#endif

        #endregion
    }

#if INC_UNITY3D_OP

    public partial struct V2
    {
        #region ##### OPERATORS #####

        public static implicit operator Vector2(V2 v2)
        {
            return new Vector2(v2.X.ToSingle(), v2.Y.ToSingle());
        }
        public static implicit operator Vector3(V2 v2)
        {
            return new Vector3(v2.X.ToSingle(), v2.Y.ToSingle(), 0);
        }

        public static implicit operator V2(Vector2 v2)
        {
            return new V2(v2.x, v2.y);
        }
        public static implicit operator V2(Vector2Int v2)
        {
            return new V2(v2.x, v2.y);
        }
        public static implicit operator V2(Vector3 v2)
        {
            return new V2(v2.x, v2.y);
        }

        #endregion
    }

#endif

#if INC_MATRIX_OP

    public partial struct V2
    {
    #region ##### OPERATORS #####

        public static V2 operator *(V2 l, Matrix m)
        {
            double X = l.X * m.M11 + l.Y * m.M21 + m.M41;
            double Y = l.X * m.M12 + l.Y * m.M22 + m.M42;
            return new V2(X, Y);
        }
        public static V2 operator *(V2 l, M4x4 m)
        {
            double X = l.X * m.E11 + l.Y * m.E21 + m.E41;
            double Y = l.X * m.E12 + l.Y * m.E22 + m.E42;
            return new V2(X, Y);
        }

    #endregion
    }

#endif

#if INC_SYSTEM_DRAWING_OP

    public partial struct V2
    {
        public static implicit operator V2(Point point)
        {
            return new V2(point);
        }

        public static implicit operator V2(Size size)
        {
            return new V2(size);
        }

    #region ##### CTORS #####

        public V2(Point p)
        {
            X = p.X;
            Y = p.Y;
        }
        public V2(Size s)
        {
            X = s.Width;
            Y = s.Height;
        }
        public V2(SizeF s)
        {
            X = s.Width;
            Y = s.Height;
        }

    #endregion

    #region ##### METHODS #####

        public System.Drawing.Point AsPoint => new Point(X.ToInt32(), Y.ToInt32());
        public System.Drawing.Size AsSize => new Size(Convert.ToInt32(X), Convert.ToInt32(Y));
        public System.Drawing.PointF AsPointF => new PointF(X.ToSingle(), Y.ToSingle());
        public System.Drawing.SizeF AsSizeF => new SizeF(X.ToSingle(), Y.ToSingle());

    #endregion
    }

#endif
}
