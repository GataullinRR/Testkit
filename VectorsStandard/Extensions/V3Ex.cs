using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Utilities;

namespace Vectors
{
    public static class V3Ex
    {
        public static V3 NextV3(this Random random)
        {
            return random.NextV3(V3.One);
        }
        public static V3 NextV3(this Random random, V3 amplitudes)
        {
            return new V3(
                random.NextDouble() * amplitudes.X, 
                random.NextDouble() * amplitudes.Y, 
                random.NextDouble() * amplitudes.Z);
        }
    }
}
