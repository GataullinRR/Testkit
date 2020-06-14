#if INC_UNITY3D_OP
using UnityEngine;
#endif
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities.Extensions;

namespace Vectors
{
    public enum CoordSystem
    {
        /// <summary>
        /// Декартовая. Оси вверх и вправо.
        /// </summary>
        BOTTOM_LEFT
    }

    public readonly struct V4
    {
        public readonly static V4 Zero = new V4(0, 0, 0, 0);
        public readonly static V4 One = new V4(1, 1, 1, 1);
        public readonly static V4 MaxSize = new V4(double.MinValue, double.MinValue, double.MaxValue, double.MaxValue);

        public readonly V2 Origin;
        public readonly V2 Destination;

        #region ##### OPERATORS #####

        public static V4 operator -(V4 l, double r)
        {
            return new V4(l.Origin.X - r, l.Origin.Y - r, l.Destination.X - r, l.Destination.Y - r);
        }
        public static V4 operator +(V4 l, double r)
        {
            return new V4(l.Origin.X + r, l.Origin.Y + r, l.Destination.X + r, l.Destination.Y + r);
        }
        public static bool operator ==(V4 l, V4 r)
        {
            return l.Origin.X == r.Origin.X && 
                l.Origin.Y == r.Origin.Y && 
                l.Destination.X == r.Destination.X && 
                l.Destination.Y == r.Destination.Y;
        }
        public static bool operator !=(V4 l, V4 r)
        {
            return !(l == r);
        }

        #endregion

        #region ##### CTOR'S #####

        public V4(double origX, double origY, V2 destination)
        {
            Origin = new V2(origX, origY);
            Destination = destination;
        }
        public V4(V2 origin, double destX, double destY)
        {
            Origin = origin;
            Destination = new V2(destX, destY);
        }
        public V4(V2 origin, V2 destination)
        {
            Origin = origin;
            Destination = destination;
        }
        public V4(double origX, double origY, double destX, double destY)
        {
            Origin = new V2(origX, origY);
            Destination = new V2(destX, destY);
        }

        #endregion

        #region ##### METHODS #####

        public V2 Size => Destination - Origin;
        public V2 Center => Origin + Size / 2;

#if INC_UNITY3D_OP
        public Rect Rect => new Rect(Origin, Size);
        public Vector4 SV4 
            => new Vector4(Origin.X.ToSingle(), Origin.Y.ToSingle(), Destination.X.ToSingle(), Destination.Y.ToSingle());
#endif

        public bool Contains(V4 rect)
        {
            return Contains(rect, false);
        }
        public bool Contains(V4 rect, bool strict)
        {
            return Contains(rect.Origin, strict) &&
                Contains(rect.Destination, strict);
        }

        public bool Contains(V2 point)
        {
            return Contains(point, false);
        }
        public bool Contains(V2 point, bool strict)
        {
            if (strict)
            {
                return point.X > Origin.X &&
                    point.X < Destination.X &&
                    point.Y > Origin.Y &&
                    point.Y < Destination.Y;
            }
            else
            {
                return point.X >= Origin.X &&
                    point.X <= Destination.X &&
                    point.Y >= Origin.Y &&
                    point.Y <= Destination.Y;
            }
        }

        /// <summary>
        /// Если фигуры не находятся в стороне друг от друга, то true
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public bool Collide(V4 rect)
        {
            return Contains(rect.Origin) || 
                Contains(rect.Origin + new V2(rect.Destination.Y, 0)) ||
                Contains(rect.Origin + new V2(0, rect.Destination.X)) ||
                Contains(rect.Origin + rect.Destination) || 
                rect.Contains(this);
        }

        /// <summary>
        /// На сколько нужно сдвинуть <paramref name="rect"/> чтобы он вошел в this. Если размер <paramref name="rect"/> больше, чем this, то выбрасывается исключение.
        /// </summary>
        /// <param name="rect"></param>
        /// <returns></returns>
        public V2 OffsetToContain(V4 rect, CoordSystem coordSystem)
        {
            if (coordSystem != CoordSystem.BOTTOM_LEFT)
            {
                throw new Exception();
            }

            double eps = 1E-9;
            if (rect.Size.X + eps > Size.X ||
                rect.Size.Y + eps > Size.Y)
            {
                throw new Exception();
            }
            else
            {
                var rectLeft = Math.Min(rect.Origin.X, rect.Destination.X);
                var rectTop = Math.Max(rect.Origin.Y, rect.Destination.Y);
                var rectRight = Math.Max(rect.Origin.X, rect.Destination.X);
                var rectBottom = Math.Min(rect.Origin.Y, rect.Destination.Y);
                var thisLeft = Math.Min(Origin.X, Destination.X);
                var thisTop = Math.Max(Origin.Y, Destination.Y);
                var thisRight = Math.Max(Origin.X, Destination.X);
                var thisBottom = Math.Min(Origin.Y, Destination.Y);
                var dLeft = rectLeft - thisLeft;
                var dTop = rectTop - thisTop;
                var dRight = rectRight - thisRight;
                var dBottom = rectBottom - thisBottom;
                var dx = 0D;
                var dy = 0D;
                dx = dLeft < 0 ? -dLeft : dx;
                dx = dRight > 0 ? -dRight : dx;
                dy = dTop > 0 ? -dTop : dy;
                dy = dBottom < 0 ? -dBottom : dy;

                return new V2(dx, dy);
            }
        }

        public override string ToString()
        {
            return $"Origin: ({Origin:F3}) Destination: ({Destination:F3}) Size: ({Size:F3})";
        }

        public override bool Equals(object obj)
        {
            if (obj is V4 v4)
            {
                return v4 == this;
            }
            else
            {
                return false;
            }
        }

        public override int GetHashCode()
        {
            return new { Origin, Destination }.GetHashCode();
        }

        #endregion
    }
}
