using Newtonsoft.Json;
using RunnerService.API;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace RunnerService.API
{
    public class GetTestsInfoRequest
    {
        public static implicit operator GGetTestsInfoRequest(GetTestsInfoRequest request)
        {
            var gRequest = new GGetTestsInfoRequest();
            gRequest.TestsIds.AddRange(request.TestsIds);

            return gRequest;
        }
        public static implicit operator GetTestsInfoRequest(GGetTestsInfoRequest request)
        {
            return new GetTestsInfoRequest(request.TestsIds.ToArray());
        }

        [Required]
        public string[] TestsIds { get; }

        public GetTestsInfoRequest(string[] testsIds)
        {
            TestsIds = testsIds ?? throw new ArgumentNullException(nameof(testsIds));
        }
    }
}
