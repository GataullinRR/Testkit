using RunnerService.APIModels;

namespace MessageHub
{
    public class TestCompletedOnSourceMessage
    {
        public string TestSourceId { get; set; }
        public RunResultBase Result { get; set; }
    }
}
