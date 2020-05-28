using RunnerService.APIModels;

namespace PresentationService.API
{
    public class TestCompletedWebMessage
    {
        public string TestName { get; set; }
        public RunResultBase RunResult { get; set; }
    };
}
