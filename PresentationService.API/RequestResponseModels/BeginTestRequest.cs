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
                TestId = request.TestIdFilter
            };

            return gRequest;
        }
        public static implicit operator BeginTestRequest(global::PresentationService.API2.GRunTestRequest request)
        {
            return new BeginTestRequest(request.TestId);
        }

        [Required]
        public string TestIdFilter { get; }

        public BeginTestRequest(string testIdFilter)
        {
            TestIdFilter = testIdFilter ?? throw new ArgumentNullException(nameof(testIdFilter));
        }
    }
}
