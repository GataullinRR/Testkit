using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utilities.Extensions;

namespace Utilities.Types
{
    public class PeriodDelay
    {
        readonly Stopwatch _stopwatch = Stopwatch.StartNew();

        public int Period { get; }
        public int TimeLeft => (Period - _stopwatch.Elapsed.TotalMilliseconds).NegativeToZero().Round();
        public bool Completed => TimeLeft == 0;

        public PeriodDelay(int period)
        {
            Period = period;
        }

        public async Task WaitTimeLeftAsync()
        {
            await WaitTimeLeftAsync(CancellationToken.None);
        }
        public async Task WaitTimeLeftAsync(CancellationToken cancellation)
        {
            await Task.Delay(TimeLeft, cancellation);
            _stopwatch.Restart();
        }
    }
}
