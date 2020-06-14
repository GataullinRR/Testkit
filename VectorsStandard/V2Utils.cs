using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities;

namespace Vectors
{
    public static class V2Utils
    {
        public static List<V2> GetPointsList(List<float> x, List<float> y)
        {
            ThrowUtils.ThrowIf_NullArgument(x, y);
            ThrowUtils.ThrowIf_True(x.Count != y.Count, "x.Count != y.Count");

            List<V2> points = new List<V2>();
            for (int i = 0; i < x.Count(); i++)
            {
                V2 point = new V2(x[i], y[i]);
                points.Add(point);
            }
            return points;
        }
    }
}
