using Newtonsoft.Json;
using Protobuf;
using System;
using System.ComponentModel.DataAnnotations;

namespace PresentationService.API
{
    public class DeleteTestRequest
    {
        public static implicit operator GDeleteTestRequest(DeleteTestRequest request)
        {
            var gRequest = new GDeleteTestRequest()
            {
                TestId = request.TestId
            };

            return gRequest;
        }
        public static implicit operator DeleteTestRequest(GDeleteTestRequest request)
        {
            return new DeleteTestRequest(request.TestId);
        }

        [Required]
        public string TestId { get; }

        public DeleteTestRequest(string testId)
        {
            TestId = testId ?? throw new ArgumentNullException(nameof(testId));
        }
    }
}
