using DDD;
using Google.Protobuf;
using Newtonsoft.Json;
using Shared.Types;
using System;
using System.ComponentModel.DataAnnotations;

namespace PresentationService.API
{
    public class ListTestsResponse
    {
        [Required]
        public TestInfo[] Tests { get; }

        [Required]
        public int TotalCount { get; }

        public ListTestsResponse(TestInfo[] tests, int totalCount)
        {
            Tests = tests ?? throw new ArgumentNullException(nameof(tests));
            TotalCount = totalCount;
        }
    }
}
