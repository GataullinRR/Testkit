using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vectors
{
    public struct V2Pair
    {
        public V2 A, B;
        
        public V2Pair(V2 a, V2 b)
        {
            A = a;
            B = b;
        }

        public double InterpolateYByX(double x)
        {
            double k = (B.Y - A.Y) / (B.X - A.X);
            double b = A.Y - k * A.X;

            return x * k + b;
        }
    }
}
