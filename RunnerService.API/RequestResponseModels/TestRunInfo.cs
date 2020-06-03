using Newtonsoft.Json;
using RunnerService.API;
using System;
using System.ComponentModel.DataAnnotations;

namespace RunnerService.API.Models
{
    public class TestRunInfo
    {
        public int TestId { get; set; }

        [Required]
        public string TestName { get; set; }

        public RunResultBase LastResult { get; set; }

        [Required]
        public StateBase State { get; set; }

        [Required]
        public RunPlanBase RunPlan { get; set; }

        public TestRunInfo(int testId, string testName, RunResultBase lastResult, StateBase state, RunPlanBase runPlan)
        {
            TestId = testId;
            TestName = testName ?? throw new ArgumentNullException(nameof(testName));
            LastResult = lastResult;
            State = state ?? throw new ArgumentNullException(nameof(state));
            RunPlan = runPlan ?? throw new ArgumentNullException(nameof(runPlan));
        }
    }
}
