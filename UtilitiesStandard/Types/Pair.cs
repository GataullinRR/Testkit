using System;
using System.Collections.Generic;
using System.Text;

namespace Utilities.Types
{
    public class Pair<T1, T2>
    {
        public T1 Value1 { get; }
        public T2 Value2 { get; }


        public Pair(T1 value1, T2 value2)
        {
            Value1 = value1;
            Value2 = value2;
        }
    }
}
