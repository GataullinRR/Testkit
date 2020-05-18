using System;

namespace Runner
{
    public interface IPredictableRunPlan
    {
        DateTime NextRun { get; }
    }
}
