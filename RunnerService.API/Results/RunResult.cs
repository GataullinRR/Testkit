namespace RunnerService.API.Models
{
    public enum RunResult
    {
        PendingCompletion,
        Passed,
        AbortedByUser,
        SUTError,
        RunnerError
    }
}
