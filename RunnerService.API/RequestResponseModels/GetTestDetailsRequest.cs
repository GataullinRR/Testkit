using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Vectors;

namespace RunnerService.API.Models
{
    public class GetTestDetailsRequest
    {
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
