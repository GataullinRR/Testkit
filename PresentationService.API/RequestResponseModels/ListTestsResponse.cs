using DDD;
using Google.Protobuf;
using Newtonsoft.Json;
using Shared.Types;
using System;
using System.ComponentModel.DataAnnotations;

namespace PresentationService.API
{
    public class ListTestsResponse : ResponseBase
    {
        static readonly JsonSerializerSettings JSON_SETTING = new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.All
        };

        public static implicit operator global::PresentationService.API2.GListTestsResponse(ListTestsResponse response)
        {
            return new API2.GListTestsResponse()
            {
                Status = response.Status,
                TestInfos = ByteString.CopyFromUtf8(
                    JsonConvert.SerializeObject(response.Tests,
                        new JsonSerializerSettings()
                        {
                            TypeNameHandling = TypeNameHandling.All
                        })),
                TotalCount = response.TotalCount
            };
        }
        public static implicit operator ListTestsResponse(global::PresentationService.API2.GListTestsResponse response)
        {
            var tests = JsonConvert.DeserializeObject<TestInfo[]>(response.TestInfos.ToStringUtf8(), JSON_SETTING);
         
            return new ListTestsResponse(tests, response.TotalCount, response.Status);
        }

        [Required]
        public TestInfo[] Tests { get; }

        public int TotalCount { get; }

        public ListTestsResponse(TestInfo[] tests, int totalCount, ResponseStatus status) : base(status)
        {
            Tests = tests ?? throw new ArgumentNullException(nameof(tests));
            TotalCount = totalCount;
        }
    }
}
