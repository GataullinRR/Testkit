namespace MessageHub
{
    public class BeginTestMessage
    {
        public int TestId { get; }
        public int ResultId { get; }
        public string TestType { get;  }
        public byte[] TestData { get; }

        public BeginTestMessage(int testId, int resultId, string testType, byte[] testData)
        {
            TestId = testId;
            ResultId = resultId;
            TestType = testType;
            TestData = testData;
        }
    }
}
