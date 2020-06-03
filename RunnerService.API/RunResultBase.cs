﻿using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

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
    }
}
