using System;

namespace MessageHub
{
    public class TestRecordedMessage
    {
        public int TestId { get; }
        public string AuthorName { get; }

        public TestRecordedMessage(int testId, string authorName)
        {
            TestId = testId;
            AuthorName = authorName ?? throw new ArgumentNullException(nameof(authorName));
        }
    }
}
