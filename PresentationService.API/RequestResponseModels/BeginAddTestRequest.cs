using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Utilities.Extensions;

namespace PresentationService.API
{
    public class BeginAddTestRequest
    {
        [Required]
        public IDictionary<string, string> TestParameters { get; }

        [JsonConstructor]
        public BeginAddTestRequest(IDictionary<string, string> testParameters)
        {
            TestParameters = testParameters ?? throw new ArgumentNullException(nameof(testParameters));
        }
    }
}
