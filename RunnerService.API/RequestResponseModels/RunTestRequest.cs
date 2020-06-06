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
        public string[] TestNameFilters { get; }
        
        [Required]
        public string UserName { get; }

        public RunTestRequest(string[] testNameFilters, string userName)
        {
            TestNameFilters = testNameFilters ?? throw new ArgumentNullException(nameof(testNameFilters));
            UserName = userName ?? throw new ArgumentNullException(nameof(userName));
        }
    }
}
