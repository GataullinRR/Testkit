using Google.Protobuf;
using Newtonsoft.Json;
using Shared.Types;
using System;
using System.ComponentModel.DataAnnotations;

namespace TestsStorageService.API
{
    public class ListTestsDataResponse
    {
        [Required]
        public TestCase[] Tests { get; }

        [Required]
        public int TotalCount { get; }

        [JsonConstructor]
        public ListTestsDataResponse(TestCase[] tests, int totalCount)
        {
            Tests = tests ?? throw new ArgumentNullException(nameof(tests));
            TotalCount = totalCount;
        }
    }
}
