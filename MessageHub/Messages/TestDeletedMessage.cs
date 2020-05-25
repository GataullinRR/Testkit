using System;

namespace MessageHub
{
    public class TestDeletedMessage
    {
        public string TestId { get; }

        public TestDeletedMessage(string testId)
        {
            TestId = testId ?? throw new ArgumentNullException(nameof(testId));
        }
    }
}
