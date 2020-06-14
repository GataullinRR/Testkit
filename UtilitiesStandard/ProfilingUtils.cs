using System;
using System.Collections.Generic;
using Utilities;
using Utilities.Extensions;
using Utilities.Types;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;

namespace Utilities
{
    public class TimeMeasurer
    {
        readonly Stopwatch _stopwatch = Stopwatch.StartNew();

        internal double TotalMillisecondsElapsed => _stopwatch.Elapsed.TotalMilliseconds;

        internal TimeMeasurer()
        {

        }

        public void Start()
        {
            _stopwatch.Start();
        }

        public void Stop()
        {
            _stopwatch.Stop();
        }
    }

    public static class ProfilingUtils
    {
        public static double MeasureExecutionTime(Action @delegate)
        {
            var sw = Stopwatch.StartNew();
            @delegate();
            return sw.Elapsed.TotalMilliseconds;
        }
        public static double MeasureExecutionTime(Action<TimeMeasurer> @delegate)
        {
            var sw = new TimeMeasurer();
            @delegate(sw);
            return sw.TotalMillisecondsElapsed;
        }
    }
}
