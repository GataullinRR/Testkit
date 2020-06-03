using System;

namespace RunnerService.API.Models
{
    public interface IPredictableRunPlan
    {
        DateTime NextRun { get; }
    }
}
