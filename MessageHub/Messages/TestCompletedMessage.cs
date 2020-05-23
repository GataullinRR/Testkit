using RunnerService.APIModels;

namespace MessageHub
{
    public class TestCompletedMessage
    {
        public string TestId { get; set; }
        public RunResultBase Result { get; set; }
    }
}
