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
                TestName = request.TestNameFilter
            };

            return gRequest;
        }
        public static implicit operator BeginTestRequest(global::PresentationService.API2.GRunTestRequest request)
        {
            return new BeginTestRequest(request.TestName);
        }

        [Required]
        public string TestNameFilter { get; }

        public BeginTestRequest(string testNameFilter)
        {
            TestNameFilter = testNameFilter ?? throw new ArgumentNullException(nameof(testNameFilter));
        }
    }
}
