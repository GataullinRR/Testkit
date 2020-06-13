namespace WebNotificationService.API
{
    public class TestResultChangedWebMessage : EntryChangedWebMessageBase
    {
        public int TestId { get; set; }
        public int ResultId { get; set; }

        public TestResultChangedWebMessage(int testId, int resultId, Change change) : base(EntryType.TestResult, change)
        {
            TestId = testId;
            ResultId = resultId;
        }
    }
}
