using System;

namespace MessageHub
{
    public class TestDeletedMessage
    {
        public int TestId { get; }
        public string? TestName { get; }

        public TestDeletedMessage(int testId, string? testName)
        {
            TestId = testId;
            TestName = testName;
        }
    }
}
