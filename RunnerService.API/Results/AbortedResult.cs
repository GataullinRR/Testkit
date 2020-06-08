namespace RunnerService.API.Models
{
    public class AbortedResult : RunResultBase
    {
        public AbortedResult() : base(RunResult.Aborted)
        {

        }
    }
}
