using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace PresentationService.API
{
    public class CancelTestRequest
    {
        [Required]
        public int TestId { get; }

        [JsonConstructor]
        public CancelTestRequest(int testId)
        {
            TestId = testId;
        }
    }
}
