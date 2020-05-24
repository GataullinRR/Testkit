using RunnerService.APIModels;

namespace PresentationService.API
{
    public class TestCompletedWebMessage
    {
        public string TestId { get; set; }
        public RunResultBase RunResult { get; set; }
    };
}
