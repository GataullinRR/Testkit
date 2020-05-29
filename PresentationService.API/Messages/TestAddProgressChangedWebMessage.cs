using System;

namespace PresentationService.API
{
    public class TestAddProgressChangedWebMessage
    {
        public string AuthorName { get; }
        public string Log { get; }

        public TestAddProgressChangedWebMessage(string authorName, string log)
        {
            AuthorName = authorName ?? throw new ArgumentNullException(nameof(authorName));
            Log = log ?? throw new ArgumentNullException(nameof(log));
        }
    };
}
