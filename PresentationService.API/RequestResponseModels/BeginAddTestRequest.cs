using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Utilities.Extensions;

namespace PresentationService.API
{
    public class BeginAddTestRequest
    {
        public static implicit operator global::PresentationService.API2.GBeginRecordingRequest(BeginAddTestRequest request)
        {
            var gRequest = new API2.GBeginRecordingRequest()
            {
                TestId = request.TestId,
                DisplayName = request.DisplayName
            };
            gRequest.Filter.AddRange(request.TestParameters);

            return gRequest;
        }
        public static implicit operator BeginAddTestRequest(global::PresentationService.API2.GBeginRecordingRequest request)
        {
            return new BeginAddTestRequest(request.TestId, request.DisplayName, request.Filter);
        }

        [Required]
        public string TestId { get; }

        [Required]
        public string DisplayName { get; }

        [Required]
        public IDictionary<string, string> TestParameters { get; }

        public BeginAddTestRequest(string testId, string displayName, IDictionary<string, string> testParameters)
        {
            TestId = testId ?? throw new ArgumentNullException(nameof(testId));
            DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
            TestParameters = testParameters ?? throw new ArgumentNullException(nameof(testParameters));
        }
    }
}
