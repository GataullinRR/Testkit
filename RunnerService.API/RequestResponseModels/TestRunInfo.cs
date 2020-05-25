using Newtonsoft.Json;
using RunnerService.APIModels;
using System;
using System.ComponentModel.DataAnnotations;

namespace RunnerService.API
{
    public class TestRunInfo
    {
        [Required]
        public string TestId { get; set; }

        public RunResultBase LastResult { get; set; }

        [Required]
        public StateBase State { get; set; }

        [Required]
        public RunPlanBase RunPlan { get; set; }

        public TestRunInfo(string testId, RunResultBase lastResult, StateBase state, RunPlanBase runPlan)
        {
            TestId = testId ?? throw new ArgumentNullException(nameof(testId));
            LastResult = lastResult;
            State = state ?? throw new ArgumentNullException(nameof(state));
            RunPlan = runPlan ?? throw new ArgumentNullException(nameof(runPlan));
        }
    }
}
