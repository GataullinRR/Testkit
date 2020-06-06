using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Vectors;

namespace PresentationService.API
{
    public class GetTestDetailsRequest
    {
        public static implicit operator global::RunnerService.API.Models.GetTestDetailsRequest (GetTestDetailsRequest request)
        {
            return new global::RunnerService.API.Models.GetTestDetailsRequest(request.TestIdFilters, request.CountFromEnd);
        }

        [Required]
        public string[] TestIdFilters { get; }

        [Required]
        public int CountFromEnd { get; }

        [JsonConstructor]
        public GetTestDetailsRequest(string[] testIdFilters, int countFromEnd)
        {
            TestIdFilters = testIdFilters ?? throw new ArgumentNullException(nameof(testIdFilters));
            CountFromEnd = countFromEnd;
        }
    }
}
