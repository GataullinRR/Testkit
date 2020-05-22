using RunnerService.APIModels;

namespace MessageHub
{
    public class TestCompletedMessage
    {
        public OperationContext OperationContext { get; set; }
        public string TestId { get; set; }
        public RunResultBase Result { get; set; }
    }
}
