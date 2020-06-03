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
        public string[] TestsIds { get; }

        [JsonConstructor]
        public GetTestsInfoRequest(string[] testsIds)
        {
            TestsIds = testsIds ?? throw new ArgumentNullException(nameof(testsIds));
        }
    }
}
