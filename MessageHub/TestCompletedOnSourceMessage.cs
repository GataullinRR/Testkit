using RunnerService.APIModels;

namespace MessageHub
{
    public class TestCompletedOnSourceMessage
    {
        public OperationContext OperationContext { get; set; }
        public string TestSourceId { get; set; }
        public RunResultBase Result { get; set; }
    }
}
