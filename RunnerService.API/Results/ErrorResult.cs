namespace RunnerService.API
{
    public abstract class ErrorResult : RunResultBase
    {
        public string Description { get; set; }
    }
}
