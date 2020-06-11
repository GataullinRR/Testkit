using System;

namespace WebNotificationService.API
{
    public class TestCancelledWebMessage
    {
        public int TestId { get; }
        public string TestName { get; }

        public TestCancelledWebMessage(int testId, string testName)
        {
            TestId = testId;
            TestName = testName;
        }
    };
}
