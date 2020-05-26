using Newtonsoft.Json;
using RunnerService.API;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace RunnerService.API
{
    public class RunTestRequest
    {
        public static implicit operator GRunTestRequest(RunTestRequest request)
        {
            var gRequest = new GRunTestRequest()
            {
                UserName = request.UserName
            };
            gRequest.TestIdsFilter.AddRange(request.TestsIdsFilter);

            return gRequest;
        }
        public static implicit operator RunTestRequest(GRunTestRequest request)
        {
            return new RunTestRequest(request.TestIdsFilter.ToArray(), request.UserName);
        }

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
