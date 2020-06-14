using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.Types;
using Utilities.Extensions;

namespace Utilities
{
    public static class MathUtils
    {
        /// <summary>
        /// 0.33333 ---> (1, 3)
        /// </summary>
        /// <param name="value"></param>
        /// <param name="accuracy">(0;1)</param>
        /// <returns></returns>
        public static (int Numerator, int Denominator) RealToFraction(double value, double accuracy)
        {
            if (accuracy <= 0.0 || accuracy >= 1.0)
            {
                throw new ArgumentOutOfRangeException("accuracy", "Must be > 0 and < 1");
            }

            int sign = Math.Sign(value);

            if (sign == -1)
            {
                value = Math.Abs(value);
            }

            // Accuracy is the maximum relative error; convert to absolute maxError
            double maxError = sign == 0 ? accuracy : value * accuracy;

            int n = (int)Math.Floor(value);
            value -= n;

            if (value < maxError)
            {
                return (sign * n, 1);
            }
            else if (1 - maxError < value)
            {
                return (sign * (n + 1), 1);
            }

            // The lower fraction is 0/1
            int lower_n = 0;
            int lower_d = 1;

            // The upper fraction is 1/1
            int upper_n = 1;
            int upper_d = 1;

            while (true)
            {
                // The middle fraction is (lower_n + upper_n) / (lower_d + upper_d)
                int middle_n = lower_n + upper_n;
                int middle_d = lower_d + upper_d;

                if (middle_d * (value + maxError) < middle_n)
                {
                    // real + error < middle : middle is our new upper
                    upper_n = middle_n;
                    upper_d = middle_d;
                }
                else if (middle_n < (value - maxError) * middle_d)
                {
                    // middle < real - error : middle is our new lower
                    lower_n = middle_n;
                    lower_d = middle_d;
                }
                else
                {
                    // Middle is our best fraction
                    return ((n * middle_d + middle_n) * sign, middle_d);
                }
            }
        }

        public static int NumOfCombinations(int numOfUniqueElements, int sizeOfGroup)
        {
            return GetKCombs(Enumerable.Range(0, numOfUniqueElements), sizeOfGroup).Count();
        }

        public static long Factorial(int number)
        {
            long result = 1;
            for (int i = 1; i < number; i++)
            {
                checked
                {
                    result *= i;
                }
            }

            return result;
        }

        public static IEnumerable<IEnumerable<T>> GetKCombs<T>(IEnumerable<T> list, int length) 
            where T : IComparable
        {
            if (length == 1)
            {
                return list.Select(t => new T[] { t });
            }
            else
            {
                return GetKCombs(list, length - 1)
                    .SelectMany(t => list.Where(o => o.CompareTo(t.Last()) > 0),
                        (t1, t2) => t1.Concat(new T[] { t2 }));
            }
        }
    }
}
