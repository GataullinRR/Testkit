using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Runner
{
    public enum RunResult
    {
        Passed,
        AbortedByUser,
        SUTError,
        RunnerError
    }

    public abstract class RunResultBase
    {
        public RunResult Result { get; set; }
        public DateTime StartTime { get; set; }
        public TimeSpan Duration { get; set; }
    }

    public class OkResult : RunResultBase
    {

    }

    public class AbortedByUserResult : RunResultBase
    {

    }

    public abstract class ErrorResult : RunResultBase
    {
        public string Description { get; set; }
    }

    public class SUTErrorResult : ErrorResult
    {

    }

    public class RunnerErrorResult : ErrorResult
    {
    }

    public class Author
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string EMail { get; set; }
        public string? Phone { get; set; }
    }

    public enum RunPlan
    {
        Manual,
        Periodic
    }

    public interface IPredictableRunPlan
    {
        DateTime NextRun { get; }
    }


    public abstract class RunPlanBase
    {
        public abstract RunPlan RunPlan { get; }
    }

    public class ManualRunPlan : RunPlanBase
    {
        public override RunPlan RunPlan => RunPlan.Manual;
    }

    public class PeriodicRunPlan : RunPlanBase, IPredictableRunPlan
    {
        public override RunPlan RunPlan => RunPlan.Periodic;

        public DateTime NextRun { get; set; }
        public TimeSpan Interval { get; set; }
    }

    public enum State
    {
        AwaitingStart,
        Suspended,
        Running
    }

    public abstract class StateBase
    {
        public abstract State State { get; }
    }

    public class AwaitingStartState : StateBase
    {
        public override State State => State.AwaitingStart;
    }

    public class SuspendedState : StateBase
    {
        public override State State => State.Suspended;
    }

    public class RunningState : StateBase
    {
        public override State State => State.Running;
    }

    public class TestInfo
    {
        public int Id { get; set; }
        public string Description { get; set; }

        public Author Author { get; set; }
        public RunPlanBase RunPlan { get; set; }
        public StateBase State { get; set; }
        public RunResultBase? LastResult { get; set; }
        public TargetBase Target { get; set; }
    }

    public enum Target
    {
        Service
    }

    public abstract class TargetBase
    {
        public abstract Target Target { get; }
        public string Id { get; set; }
    }

    public class ServiceTestTarget : TargetBase
    {
        public override Target Target => Target.Service;

        public int ServiceId { get; set; }
        public string Name { get; set; }
    }
}
