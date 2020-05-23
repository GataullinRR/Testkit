using RunnerService.APIModels;

namespace MessageHub
{
    public class TestCompletedOnSourceMessage
    {
        public string TestId { get; set; }
        public RunResultBase Result { get; set; }
    }
}
