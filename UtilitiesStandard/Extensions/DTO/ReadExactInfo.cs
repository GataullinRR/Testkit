using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.Extensions;

namespace Utilities.Extensions
{
    public class ReadExactInfo
    {
        public int MethodTimeout { get; }
        public int StreamReadTimeout { get; }
        public bool TimeoutOnlyIfNothingWasRead { get; }
        public int RetryDelay { get; }
        public bool ThrowIfZeroReceived { get; }

        public ReadExactInfo()
            : this(-1)
        {

        }
        public ReadExactInfo(int methodTimeout)
            : this(methodTimeout, false)
        {

        }
        public ReadExactInfo(int methodTimeout, bool timeoutOnlyIfNothingWasRead)
            : this(methodTimeout, 0, timeoutOnlyIfNothingWasRead, 0, false)
        {

        }
        public ReadExactInfo
            (int methodTimeout, int streamReadTimeout, bool timeoutOnlyIfNothingWasRead, int retryDelay, bool throwIfZeroReceived)
        {
            MethodTimeout = methodTimeout;
            StreamReadTimeout = streamReadTimeout;
            TimeoutOnlyIfNothingWasRead = timeoutOnlyIfNothingWasRead;
            RetryDelay = retryDelay;
            ThrowIfZeroReceived = throwIfZeroReceived;
        }
    }
}
