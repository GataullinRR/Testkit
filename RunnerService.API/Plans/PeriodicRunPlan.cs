using System;

namespace RunnerService.APIModels
{
    public class PeriodicRunPlan : RunPlanBase, IPredictableRunPlan
    {
        public DateTime NextRun { get; set; }
        public TimeSpan Interval { get; set; }
        public double RepeatIntervalCoefficient { get; set; } = 0.1;

        public PeriodicRunPlan() : base(RunPlan.Periodic)
        {

        }
    }
}
