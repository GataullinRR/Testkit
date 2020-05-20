namespace RunnerService.API
{
    public class ManualRunPlan : RunPlanBase
    {
        public override RunPlan RunPlan => RunPlan.Manual;
    }
}
