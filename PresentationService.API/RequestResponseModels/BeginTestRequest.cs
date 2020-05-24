using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace PresentationService.API
{
    public class BeginTestRequest
    {
        public static implicit operator global::PresentationService.API2.GRunTestRequest(BeginTestRequest request)
        {
            var gRequest = new API2.GRunTestRequest()
            {
                TestId = request.TestId
            };

            return gRequest;
        }
        public static implicit operator BeginTestRequest(global::PresentationService.API2.GRunTestRequest request)
        {
            return new BeginTestRequest(request.TestId);
        }

        [Required]
        public string TestId { get; }

        public BeginTestRequest(string testId)
        {
            TestId = testId ?? throw new ArgumentNullException(nameof(testId));
        }
    }
}
