using Shared.Types;
using System.ComponentModel.DataAnnotations;

namespace PresentationService.API
{
    public class GetTestsAddStateResponse
    {
        [Required]
        public bool HasBegan { get; set; }

        public GetTestsAddStateResponse(bool hasBegan)
        {
            HasBegan = hasBegan;
        }
    }
}
