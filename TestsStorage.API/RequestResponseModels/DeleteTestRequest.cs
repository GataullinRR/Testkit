using Newtonsoft.Json;
using Protobuf;
using System;
using System.ComponentModel.DataAnnotations;
using Utilities.Extensions;

namespace TestsStorageService.API
{
    public class DeleteTestRequest
    {
        public bool IsById { get; }
        public int TestId { get; }

        public string? TestNameFilter { get; } 

        public DeleteTestRequest(int testId)
        {
            IsById = true;
            TestId = testId;
        }
        public DeleteTestRequest(string testNameFilter)
        {
            TestNameFilter = testNameFilter ?? throw new ArgumentNullException(nameof(testNameFilter));
        }

        [JsonConstructor]
        DeleteTestRequest(bool isById, int testId, string? testNameFilter)
        {
            IsById = isById;
            TestId = testId;
            TestNameFilter = testNameFilter;
        }
    }
}
