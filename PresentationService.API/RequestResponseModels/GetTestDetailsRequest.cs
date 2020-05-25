using Newtonsoft.Json;
using Protobuf;
using System;
using System.ComponentModel.DataAnnotations;
using Vectors;

namespace PresentationService.API
{
    public class GetTestDetailsRequest
    {
        public static implicit operator global::Protobuf.GGetTestDetailsRequest(GetTestDetailsRequest request)
        {
            return new Protobuf.GGetTestDetailsRequest()
            {
                CountFromEnd = request.CountFromEnd,
                TestId = request.TestId
            };
        }
        public static implicit operator GetTestDetailsRequest(global::Protobuf.GGetTestDetailsRequest request)
        {
            return new GetTestDetailsRequest(request.TestId, request.CountFromEnd);
        }

        [Required]
        public string TestId { get; }

        public int CountFromEnd { get; }

        public GetTestDetailsRequest(string testId, int countFromEnd)
        {
            TestId = testId ?? throw new ArgumentNullException(nameof(testId));
            CountFromEnd = countFromEnd;
        }
    }
}
