using System;

namespace MessageHub
{
    public class BeginTestMessage
    {
        public string TestId { get; }
        public int ResultId { get; }
        public string TestType { get;  }
        public byte[] TestData { get; }

        public BeginTestMessage(string testId, int resultId, string testType, byte[] testData)
        {
            TestId = testId;
            ResultId = resultId;
            TestType = testType;
            TestData = testData;
        }
    }
}
