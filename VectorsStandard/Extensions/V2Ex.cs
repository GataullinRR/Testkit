using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities;

namespace Vectors
{
    public static class V2Ex
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sourceValues"></param>
        /// <param name="pointProvider">current source sequence element index; current element value</param>
        /// <returns></returns>
        public static List<V2> ToPointsList(this List<float> sourceValues, Func<int, float, V2> pointProvider)
        {
            ThrowUtils.ThrowIf_NullArgument(sourceValues);
            ThrowUtils.ThrowIf_NullArgument(pointProvider);

            List<V2> points = new List<V2>();
            for (int i = 0; i < sourceValues.Count(); i++)
            {
                V2 point = pointProvider(i, sourceValues[i]);
                points.Add(point);
            }
            return points;
        }

        public static double MaxX(this IEnumerable<V2> vectors)
        {
            return vectors.Max(v => v.X);
        }
        public static double MaxY(this IEnumerable<V2> vectors)
        {
            return vectors.Max(v => v.Y);
        }
        public static double MinX(this IEnumerable<V2> vectors)
        {
            return vectors.Min(v => v.X);
        }
        public static double MinY(this IEnumerable<V2> vectors)
        {
            return vectors.Min(v => v.Y);
        }
    }
}
