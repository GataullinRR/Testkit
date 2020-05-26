using Newtonsoft.Json;
using Protobuf;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Vectors;

namespace PresentationService.API
{
    public class GetTestDetailsRequest
    {
        public static implicit operator global::Protobuf.GGetTestDetailsRequest(GetTestDetailsRequest request)
        {
            var gRequest = new Protobuf.GGetTestDetailsRequest()
            {
                CountFromEnd = request.CountFromEnd,
            };
            gRequest.TestIdFilters.Add(request.TestIdFilters);

            return gRequest;
        }
        public static implicit operator GetTestDetailsRequest(global::Protobuf.GGetTestDetailsRequest request)
        {
            return new GetTestDetailsRequest(request.TestIdFilters.ToArray(), request.CountFromEnd);
        }

        [Required]
        public string[] TestIdFilters { get; }

        public int CountFromEnd { get; }

        public GetTestDetailsRequest(string[] testIdFilters, int countFromEnd)
        {
            TestIdFilters = testIdFilters ?? throw new ArgumentNullException(nameof(testIdFilters));
            CountFromEnd = countFromEnd;
        }
    }
}
