using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace PresentationService.API
{
    public class BeginTestRequest
    {
        [Required]
        public string TestNameFilter { get; }

        [JsonConstructor]
        public BeginTestRequest(string testNameFilter)
        {
            TestNameFilter = testNameFilter ?? throw new ArgumentNullException(nameof(testNameFilter));
        }
    }
}
