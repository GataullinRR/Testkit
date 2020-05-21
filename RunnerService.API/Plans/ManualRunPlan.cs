namespace RunnerService.APIModels
{
    public class ManualRunPlan : RunPlanBase
    {
        public override RunPlan RunPlan => RunPlan.Manual;
    }
}
