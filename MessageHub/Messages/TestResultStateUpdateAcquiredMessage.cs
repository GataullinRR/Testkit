using RunnerService.API.Models;
using System;
using System.Collections.Generic;

namespace MessageHub
{
    public class TestResultStateAcquiredMessage
    {
        public int ResultId { get; }
        public StateInfo NewState { get; }

        public TestResultStateAcquiredMessage(int resultId, StateInfo newState)
        {
            ResultId = resultId;
            NewState = newState ?? throw new ArgumentNullException(nameof(newState));
        }
    }
}
