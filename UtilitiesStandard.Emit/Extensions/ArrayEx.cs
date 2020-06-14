using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;

namespace Utilities.Extensions
{
    public static class ArrayEx
    {
        public static IEnumerable<T> SumDynamically<T>(this IEnumerable<T> a1, IEnumerable<T> a2)
        {
            var t = typeof(T);
            var addOpType = typeof(Func<T, T, T>);
            Func<T, T, T> add;
            if (t.IsPrimitive)
            {
                var adder = new DynamicMethod("_", t, new[] { t, t });
                var gen = adder.GetILGenerator();
                gen.Emit(OpCodes.Ldarg_0);
                gen.Emit(OpCodes.Ldarg_1);
                gen.Emit(OpCodes.Add);
                gen.Emit(OpCodes.Ret);
                add = (Func<T, T, T>)adder.CreateDelegate(addOpType);
            }
            else
            {
                add = (Func<T, T, T>)typeof(T).GetMethod("op_Addition").CreateDelegate(typeof(Func<T, T, T>));
            }

            return a1.Zip(a2, (v1, v2) => add(v1, v2));
        }
    }
}
