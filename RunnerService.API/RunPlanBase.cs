namespace RunnerService.APIModels
{
    public abstract class RunPlanBase
    {
        protected RunPlanBase(RunPlan runPlan)
        {
            RunPlan = runPlan;
        }

        public RunPlan RunPlan { get; set; }
    }
}
