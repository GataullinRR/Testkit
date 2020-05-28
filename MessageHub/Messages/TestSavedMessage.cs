using System;

namespace MessageHub
{
    public class TestAddedMessage
    {
        public int TestId { get; }
        public string? TestName { get; }
        public string AuthorName { get; }

        public TestAddedMessage(int testId, string testName, string authorName)
        {
            TestId = testId;
            TestName = testName;
            AuthorName = authorName ?? throw new ArgumentNullException(nameof(authorName));
        }
    }
}
