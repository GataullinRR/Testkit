using RunnerService.API.Models;
using System;

namespace MessageHub
{
    public class TestResultStateUpdatedMessage
    {
        public int TestId { get; }
        public int ResultId { get; }
        public StateInfo NewState { get; }

        public TestResultStateUpdatedMessage(int testId, int resultId, StateInfo newState)
        {
            TestId = testId;
            ResultId = resultId;
            NewState = newState ?? throw new ArgumentNullException(nameof(newState));
        }
    }
}
