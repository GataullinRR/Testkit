using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace RunnerService.APIModels
{
    public abstract class RunResultBase
    {
        protected RunResultBase(RunResult result)
        {
            Result = result;
        }

        public RunResult Result { get; set; }
        public DateTime StartTime { get; set; }
        public TimeSpan Duration { get; set; }
    }
}
