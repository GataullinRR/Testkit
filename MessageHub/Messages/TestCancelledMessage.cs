namespace MessageHub
{
    public class TestCancelledMessage
    {
        public int TestId { get; }
        public int ResultId { get; }
        public string TestName { get; }
        public string StartedByUser { get; }

        public TestCancelledMessage(int testId, int resultId, string testName, string startedByUser)
        {
            TestId = testId;
            ResultId = resultId;
            TestName = testName;
            StartedByUser = startedByUser;
        }
    }
}
