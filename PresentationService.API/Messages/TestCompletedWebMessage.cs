using RunnerService.APIModels;
using System;

namespace PresentationService.API
{
    public class TestCompletedWebMessage
    {
        public string TestName { get; set; }
        public RunResultBase RunResult { get; set; }

        public TestCompletedWebMessage(string testName, RunResultBase runResult)
        {
            TestName = testName ?? throw new ArgumentNullException(nameof(testName));
            RunResult = runResult ?? throw new ArgumentNullException(nameof(runResult));
        }
    };
}
