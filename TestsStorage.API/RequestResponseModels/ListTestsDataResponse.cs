using Google.Protobuf;
using Newtonsoft.Json;
using PresentationService.API2;
using Shared.Types;
using System;
using System.ComponentModel.DataAnnotations;

namespace TestsStorageService.API
{
    public class ListTestsDataResponse : ResponseBase
    {
        static readonly JsonSerializerSettings JSON_SETTING = new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.All
        };

        public static implicit operator GListTestsDataResponse(ListTestsDataResponse response)
        {
            return new GListTestsDataResponse()
            {
                Status = response.Status,
                Tests = ByteString.CopyFromUtf8(
                    JsonConvert.SerializeObject(response.Tests,
                        new JsonSerializerSettings()
                        {
                            TypeNameHandling = TypeNameHandling.All
                        })),
                Count = (uint)response.TotalCount
            };
        }
        public static implicit operator ListTestsDataResponse(GListTestsDataResponse response)
        {
            var tests = JsonConvert.DeserializeObject<TestCase[]>(response.Tests.ToStringUtf8(), JSON_SETTING);
         
            return new ListTestsDataResponse(tests, (int)response.Count, response.Status);
        }

        [Required]
        public TestCase[] Tests { get; }
        public int TotalCount { get; }

        public ListTestsDataResponse(TestCase[] tests, int totalCount, ResponseStatus status) : base(status)
        {
            Tests = tests ?? throw new ArgumentNullException(nameof(tests));
            TotalCount = totalCount;
        }
    }
}
