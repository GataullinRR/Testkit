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
    public class MiscUtils
    {
        public static IEnumerable<T> TupleToEnumerable<T>(object tuple)
        {
            if (tuple.GetType().GetInterface("ITupleInternal") != null)
            {
                foreach (var prop in tuple.GetType()
                    .GetProperties()
                    .Where(x => x.Name.StartsWith("Item")))
                {
                    yield return (T)prop.GetValue(tuple);
                }
            }
            else
            {
                throw new ArgumentException("Not a tuple!");
            }
        }
    }
}
