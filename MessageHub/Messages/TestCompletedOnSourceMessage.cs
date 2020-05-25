using RunnerService.APIModels;
using System;

namespace MessageHub
{
    public class TestCompletedOnSourceMessage
    {
        public string TestId { get; }
        public int ResultId { get; }
        public RunResultBase Result { get; }

        public TestCompletedOnSourceMessage(string testId, int resultId, RunResultBase result)
        {
            TestId = testId ?? throw new ArgumentNullException(nameof(testId));
            ResultId = resultId;
            Result = result ?? throw new ArgumentNullException(nameof(result));
        }
    }
}
