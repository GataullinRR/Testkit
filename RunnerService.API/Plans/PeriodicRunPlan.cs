using System;

namespace RunnerService.APIModels
{
    public class PeriodicRunPlan : RunPlanBase, IPredictableRunPlan
    {
        public override RunPlan RunPlan => RunPlan.Periodic;

        public DateTime NextRun { get; set; }
        public TimeSpan Interval { get; set; }
    }
}
