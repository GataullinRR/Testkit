using System;

namespace RunnerService.API
{
    public interface IPredictableRunPlan
    {
        DateTime NextRun { get; }
    }
}
