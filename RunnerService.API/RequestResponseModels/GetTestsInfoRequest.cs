using Newtonsoft.Json;
using RunnerService.API;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace RunnerService.API.Models
{
    public class GetTestsInfoRequest
    {
        [Required]
        public string[] TestNames { get; }

        [JsonConstructor]
        public GetTestsInfoRequest(string[] testNames)
        {
            TestNames = testNames ?? throw new ArgumentNullException(nameof(testNames));
        }
    }
}
