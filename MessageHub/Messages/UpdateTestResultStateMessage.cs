using RunnerService.API.Models;
using System;

namespace MessageHub
{
    public class UpdateTestResultStateMessage
    {
        public int TestId { get; }
        public int ResultId { get; }
        public string? SourceId { get; }
        public StateInfo CurrentState { get; }

        public UpdateTestResultStateMessage(int testId, int resultId, string? sourceId, StateInfo currentState)
        {
            TestId = testId;
            ResultId = resultId;
            SourceId = sourceId;
            CurrentState = currentState ?? throw new ArgumentNullException(nameof(currentState));
        }
    }
}
