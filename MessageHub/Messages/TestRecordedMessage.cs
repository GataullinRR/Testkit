using System;

namespace MessageHub
{
    /// <summary>
    /// Added to Db, but not saved
    /// </summary>
    public class TestRecordedMessage
    {
        public int TestId { get; }
        public string? TestName { get; }
        public string? TestDescription { get; }

        public TestRecordedMessage(int testId, string? testName, string? testDescription)
        {
            TestId = testId;
            TestName = testName;
            TestDescription = testDescription;
        }
    }
}
