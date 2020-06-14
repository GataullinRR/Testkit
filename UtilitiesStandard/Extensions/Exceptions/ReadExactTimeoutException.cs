using System;
using System.Collections.Generic;

namespace Utilities.Extensions
{
    public class ReadExactTimeoutException : TimeoutException
    {
        public IEnumerable<byte> Buffer { get; }
        public int Required { get; }

        public ReadExactTimeoutException(IEnumerable<byte> buffer, int required)
        {
            Buffer = buffer;
            Required = required;
        }
    }
}
