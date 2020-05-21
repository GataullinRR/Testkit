using System;

namespace RunnerService.APIModels
{
    public interface IPredictableRunPlan
    {
        DateTime NextRun { get; }
    }
}
