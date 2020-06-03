namespace RunnerService.API.Models
{
    public abstract class ErrorResult : RunResultBase
    {
        protected ErrorResult(RunResult result) : base(result)
        {

        }

        public string Description { get; set; }
    }
}
