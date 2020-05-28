using Shared.Types;
using System.ComponentModel.DataAnnotations;

namespace PresentationService.API
{
    public class GetTestsAddStateResponse : ResponseBase
    {
        public static implicit operator global::PresentationService.API2.GGetTestsAddStateResponse(GetTestsAddStateResponse response)
        {
            return new global::PresentationService.API2.GGetTestsAddStateResponse()
            {
                HasBegan = response.HasBegan,
                Status = response.Status
            };
        }
        public static implicit operator GetTestsAddStateResponse(global::PresentationService.API2.GGetTestsAddStateResponse response)
        {
            return new GetTestsAddStateResponse(response.HasBegan, response.Status);
        }

        [Required]
        public bool HasBegan { get; set; }

        public GetTestsAddStateResponse(bool hasBegan, ResponseStatus status) : base(status)
        {
            HasBegan = hasBegan;
        }
    }
}
