using Newtonsoft.Json;
using RunnerService.API;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace RunnerService.API.Models
{
    public class RunTestRequest
    {
        [Required]
        public string[] TestsIdsFilter { get; }
        
        [Required]
        public string UserName { get; set; }

        public RunTestRequest(string[] testsIdsFilter, string userName)
        {
            TestsIdsFilter = testsIdsFilter ?? throw new ArgumentNullException(nameof(testsIdsFilter));
            UserName = userName ?? throw new ArgumentNullException(nameof(userName));
        }
    }
}
