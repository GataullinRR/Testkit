using System;

namespace RunnerService.API
{
    public abstract class RunResultBase
    {
        public RunResult Result { get; set; }
        public DateTime StartTime { get; set; }
        public TimeSpan Duration { get; set; }
    }
}
