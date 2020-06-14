namespace RunnerService.API.Models
{
    public enum RunResult
    {
        Running = 0,
        Aborted = 1000,
        Passed = 2000,
        Error = 3000,
        FatalError = 4000
    }
}
