using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace PresentationService.API
{
    public class SaveRecordedTestRequest
    {
        public static implicit operator global::PresentationService.API2.GSaveRecordedTestRequest(SaveRecordedTestRequest request)
        {
            var gRequest = new API2.GSaveRecordedTestRequest()
            {
                TestId = request.TestId,
                TestName = request.TestName,
                DisplayName = request.TestDescription,
            };

            return gRequest;
        }
        public static implicit operator SaveRecordedTestRequest(global::PresentationService.API2.GSaveRecordedTestRequest request)
        {
            return new SaveRecordedTestRequest(request.TestId, request.TestName, request.DisplayName);
        }

        [Required]
        public int TestId { get; }

        [Required]
        public string TestName { get; }

        [Required]
        public string TestDescription { get; }

        public SaveRecordedTestRequest(int testId, string testName, string testDescription)
        {
            TestId = testId;
            TestName = testName ?? throw new ArgumentNullException(nameof(testName));
            TestDescription = testDescription ?? throw new ArgumentNullException(nameof(testDescription));
        }
    }
}
