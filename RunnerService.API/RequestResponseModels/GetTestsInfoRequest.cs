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
        public int[] TestIds { get; }

        [JsonConstructor]
        public GetTestsInfoRequest(int[] testIds)
        {
            TestIds = testIds ?? throw new ArgumentNullException(nameof(testIds));
        }
    }
}
