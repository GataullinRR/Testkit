namespace RunnerService.API.Models
{
    public class RunnerErrorResult : ErrorResult
    {
        public RunnerErrorResult() : base(RunResult.RunnerError)
        {

        }
    }
}
