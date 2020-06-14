using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
#if INC_UNITY3D_OP
using UnityEngine;
#endif
using Utilities;
using Utilities.Extensions;

namespace Vectors
{
    public readonly partial struct V3
    {
        public static readonly V3 Zero = new V3(0);
        public static readonly V3 One = new V3(1);
        public static readonly V3 NaN = new V3(double.NaN);

        public readonly double X;
        public readonly double Y;
        public readonly double Z;

#region ##### OPERATORS #####

        public static V3 operator -(V3 v1, V3 v2)
        {
            return new V3(v1.X - v2.X, v1.Y - v2.Y, v1.Z - v2.Z);
        }
        public static V3 operator -(V3 l, float r)
        {
            return new V3(l.X - r, l.Y - r, l.Z - r);
        }
        public static V3 operator -(V3 l, int r)
        {
            return new V3(l.X - r, l.Y - r, l.Z - r);
        }
        public static V3 operator -(V3 l, double r)
        {
            return new V3(l.X - r, l.Y - r, l.Z - r);
        }

        public static V3 operator +(V3 v1, V3 v2)
        {
            return new V3(v1.X + v2.X, v1.Y + v2.Y, v1.Z + v2.Z);
        }
        public static V3 operator +(V3 l, float r)
        {
            return new V3(l.X + r, l.Y + r, l.Z + r);
        }
        public static V3 operator +(V3 l, int r)
        {
            return new V3(l.X + r, l.Y + r, l.Z + r);
        }
        public static V3 operator +(V3 l, double r)
        {
            return new V3(l.X + r, l.Y + r, l.Z + r);
        }

        public static V3 operator /(V3 v1, V3 v2)
        {
            return new V3(v1.X / v2.X, v1.Y / v2.Y, v1.Z / v2.Z);
        }
        public static V3 operator /(V3 l, float r)
        {
            return new V3(l.X / r, l.Y / r, l.Z / r);
        }
        public static V3 operator /(V3 l, int r)
        {
            return new V3(l.X / r, l.Y / r, l.Z / r);
        }
        public static V3 operator /(V3 l, double r)
        {
            return new V3(l.X / r, l.Y / r, l.Z / r);
        }

        public static V3 operator *(V3 v1, V3 v2)
        {
            return new V3(v1.X * v2.X, v1.Y * v2.Y, v1.Z * v2.Z);
        }
        public static V3 operator *(V3 l, float r)
        {
            return new V3(l.X * r, l.Y * r, l.Z * r);
        }
        public static V3 operator *(V3 l, int r)
        {
            return new V3(l.X * r, l.Y * r, l.Z * r);
        }
        public static V3 operator *(V3 l, double r)
        {
            return new V3(l.X * r, l.Y * r, l.Z * r);
        }

        public static bool operator ==(V3 l, V3 r)
        {
            return l.X == r.X && l.Y == r.Y && l.Z == r.Z;
        }
        public static bool operator !=(V3 l, V3 r)
        {
            return !(l == r);
        }

#endregion

#region ##### CTOR'S #####

        public V3(float v)
        {
            X = v;
            Y = v;
            Z = v;
        }
        public V3(int v)
        {
            X = v;
            Y = v;
            Z = v;
        }
        public V3(double v)
        {
            X = v;
            Y = v;
            Z = v;
        }

        public V3(V2 xy)
        {
            X = xy.X;
            Y = xy.Y;
            Z = 0;
        }

        public V3(float x, V2 yz)
        {
            X = x;
            Y = yz.X;
            Z = yz.Y;
        }
        public V3(double x, V2 yz)
        {
            X = x;
            Y = yz.X;
            Z = yz.Y;
        }
        public V3(V2 xy, float z)
        {
            X = xy.X;
            Y = xy.Y;
            Z = z;
        }
        public V3(V2 xy, double z)
        {
            X = xy.X;
            Y = xy.Y;
            Z = z;
        }
        public V3(float x, float y, float z)
        {
            X = x;
            Y = y;
            Z = z;
        }
        public V3(double x, double y, double z)
        {
            X = x;
            Y = y;
            Z = z;
        }

#endregion

#region ##### SET #####

        public V3 SetX(int value)
        {
            return new V3(value, Y, Z);
        }
        public V3 SetX(float value)
        {
            return new V3(value, Y, Z);
        }
        public V3 SetX(double value)
        {
            return new V3(value, Y, Z);
        }

        public V3 SetY(int value)
        {
            return new V3(X, value, Z);
        }
        public V3 SetY(float value)
        {
            return new V3(X, value, Z);
        }
        public V3 SetY(double value)
        {
            return new V3(X, value, Z);
        }

        public V3 SetZ(int value)
        {
            return new V3(X, Y, value);
        }
        public V3 SetZ(float value)
        {
            return new V3(X, Y, value);
        }
        public V3 SetZ(double value)
        {
            return new V3(X, Y, value);
        }

#endregion

#region ##### METHODS #####

        public V2 XY => new V2(X, Y);
        public double Mag => Math.Sqrt(X.Pow(2) + Y.Pow(2) + Z.Pow(2));
        public double[] AsArray => new double[] { X, Y, Z };
        public V3 Abs => new V3(X.Abs(), Y.Abs(), Z.Abs());

        #endregion

        #region ##### OBJECT OVERRIDE #####

        public override bool Equals(object obj)
        {
            if (obj is V3)
            {
                return Equals((V3)obj);
            }
            else
            {
                return false;
            }
        }
        public bool Equals(V3 v3)
        {
            return v3.X == X && 
                   v3.Y == Y && 
                   v3.Z == Z;
        }

        public override int GetHashCode()
        {
            return new { X, Y, Z }.GetHashCode();
        }

        public override string ToString()
        {
            return ToString("F4");
        }
        public string ToString(string format)
        {
            return string.Format($"X: {X.ToString(format)} Y: {Y.ToString(format)} Z: {Z.ToString(format)}");
        }

        public static V3 Parse(string str)
        {
            var substrs = str.Split(' ', ':');
            double x = double.Parse(substrs[2]);
            double y = double.Parse(substrs[5]);
            double z = double.Parse(substrs[8]);
            return new V3(x, y, z);
        }

#endregion
    }

#if INC_UNITY3D_OP

    public partial struct V3
    {
#region ##### OPERATORS #####

        public static implicit operator Vector2(V3 v3)
        {
            return new Vector2(v3.X.ToSingle(), v3.Y.ToSingle());
        }
        public static implicit operator Vector3(V3 v3)
        {
            return new Vector3(v3.X.ToSingle(), v3.Y.ToSingle(), v3.Z.ToSingle());
        }

        public static implicit operator V3(Vector2 v2)
        {
            return new V3(v2.x, v2.y, 0);
        }
        public static implicit operator V3(Vector3Int v3)
        {
            return new V3(v3.x, v3.y, v3.z);
        }
        public static implicit operator V3(Vector3 v3)
        {
            return new V3(v3.x, v3.y, v3.z);
        }

#endregion
    }

#endif
}
