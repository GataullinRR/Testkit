namespace RunnerService.API.Models
{
    public enum RunResult
    {
        Running = 0,
        Aborted = 1000,
        Passed = 2000,
        SUTError = 3000,
        RunnerError = 4000
    }
}
