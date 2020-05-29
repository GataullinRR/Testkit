using System;

namespace PresentationService.API
{
    public class TestRecordedWebMessage
    {
        public int TestId { get; }
        public string AuthorName { get; }

        public TestRecordedWebMessage(int testId, string authorName)
        {
            TestId = testId;
            AuthorName = authorName ?? throw new ArgumentNullException(nameof(authorName));
        }
    };
}
