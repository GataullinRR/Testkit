using System;

namespace PresentationService.API
{
    public class TestAddedWebMessage
    {
        public int TestId { get; }
        public string? TestName { get; }
        public string AuthorName { get; }

        public TestAddedWebMessage(int testId, string? testName, string authorName)
        {
            TestId = testId;
            TestName = testName;
            AuthorName = authorName ?? throw new ArgumentNullException(nameof(authorName));
        }
    };
}
