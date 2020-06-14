using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Utilities.Types
{
    /// <summary>
    /// Non-blocking
    /// </summary>
    public struct ThreadSafeBool
    {
        public static implicit operator bool (ThreadSafeBool boolean)
        {
            return boolean.Value;
        }

        // default is false, set 1 for true.
        int _threadSafeBoolBackValue;

        public bool Value
        {
            get => Interlocked.CompareExchange(ref _threadSafeBoolBackValue, 1, 1) == 1;
            set
            {
                if (value)
                {
                    Interlocked.CompareExchange(ref _threadSafeBoolBackValue, 1, 0);
                }
                else
                {
                    Interlocked.CompareExchange(ref _threadSafeBoolBackValue, 0, 1);
                }
            }
        }
    }
}
