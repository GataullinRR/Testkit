using RunnerService.APIModels;
using System;

namespace MessageHub
{
    public class TestCompletedOnSourceMessage
    {
        public int TestId { get; }
        public int ResultId { get; }
        public RunResultBase Result { get; }

        public TestCompletedOnSourceMessage(int testId, int resultId, RunResultBase result)
        {
            TestId = testId;
            ResultId = resultId;
            Result = result ?? throw new ArgumentNullException(nameof(result));
        }
    }
}
