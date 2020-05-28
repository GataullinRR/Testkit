using Newtonsoft.Json;
using Protobuf;
using System;
using System.ComponentModel.DataAnnotations;
using Utilities.Extensions;

namespace PresentationService.API
{
    public class DeleteTestRequest
    {
        public static implicit operator GDeleteTestRequest(DeleteTestRequest request)
        {
            var gRequest = new GDeleteTestRequest();

            if (request.TestNameFilter == null)
            {
                gRequest.TestId = request.TestId;
            }
            else
            {
                gRequest.TestNameFilter = request.TestNameFilter;
            }

            return gRequest;
        }
        public static implicit operator DeleteTestRequest(GDeleteTestRequest request)
        {
            if (request.TestNameFilter.IsNotNullOrEmpty())
            {
                return new DeleteTestRequest(request.TestNameFilter);
            }
            else
            {
                return new DeleteTestRequest(request.TestId);
            }
        }

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
    }
}
