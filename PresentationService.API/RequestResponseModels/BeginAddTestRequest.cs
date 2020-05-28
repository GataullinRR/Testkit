using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Utilities.Extensions;

namespace PresentationService.API
{
    public class BeginAddTestRequest
    {
        public static implicit operator global::PresentationService.API2.GBeginAddTestRequest(BeginAddTestRequest request)
        {
            var gRequest = new API2.GBeginAddTestRequest()
            {

            };
            gRequest.Filter.AddRange(request.TestParameters);

            return gRequest;
        }
        public static implicit operator BeginAddTestRequest(global::PresentationService.API2.GBeginAddTestRequest request)
        {
            return new BeginAddTestRequest(request.Filter);
        }

        [Required]
        public IDictionary<string, string> TestParameters { get; }

        public BeginAddTestRequest(IDictionary<string, string> testParameters)
        {
            TestParameters = testParameters ?? throw new ArgumentNullException(nameof(testParameters));
        }
    }
}
