using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Utilities
{
    public static class TaskUtils
    {
        public static async Task<double> DelayAndMeasure(int delayInMilliseconds, CancellationToken cancellationToken)
        {
            var sw = Stopwatch.StartNew();
            await Task.Delay(delayInMilliseconds, cancellationToken);
            return sw.Elapsed.TotalMilliseconds;
        }
    }
}
