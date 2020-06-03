using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace PresentationService.API
{
    public class SaveRecordedTestRequest
    {
        [Required]
        public int TestId { get; }

        [Required]
        public string TestName { get; }

        [Required]
        public string TestDescription { get; }

        [JsonConstructor]
        public SaveRecordedTestRequest(int testId, string testName, string testDescription)
        {
            TestId = testId;
            TestName = testName ?? throw new ArgumentNullException(nameof(testName));
            TestDescription = testDescription ?? throw new ArgumentNullException(nameof(testDescription));
        }
    }
}
