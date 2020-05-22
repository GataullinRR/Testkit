using TestsStorageService.API;

namespace MessageHub
{
    public class TestAcquiredMessage
    {
        public OperationContext OperationContext { get; set; }
        public CSTestCaseInfo Test { get; set; }
    }
}
