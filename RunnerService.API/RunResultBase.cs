using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using Utilities.Types;

namespace RunnerService.API.Models
{
    public abstract class RunResultBase
    {
        protected RunResultBase(RunResult result)
        {
            Result = result;
        }

        public int Id { get; set; }
        public int TestId { get; set; }
        public string TestName { get; set; }
        public RunResult Result { get; set; }
        public DateTime StartTime { get; set; }
        public TimeSpan Duration { get; set; }
        public string StartedByUser { get; set; }

        public string? ExpectedParameters { get; set; }
        public string? ActualParameters { get; set; }
        public string? SourceId { get; set; } 
        [Include(EntityGroups.ALL, EntityGroups.RESULTS)]
        public StateInfo State { get; set; }
    }
}
