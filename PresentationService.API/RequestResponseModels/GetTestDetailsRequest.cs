using Newtonsoft.Json;
using SharedT.Types;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Vectors;

namespace PresentationService.API
{
    public class GetTestDetailsRequest
    {
        public static implicit operator global::RunnerService.API.Models.GetTestDetailsRequest (GetTestDetailsRequest request)
        {
            return new global::RunnerService.API.Models.GetTestDetailsRequest(request.CountFromEnd, request.FilteringOrders);
        }

        [Required]
        public IFilterOrder[] FilteringOrders { get; }

        [Required]
        public int CountFromEnd { get; }

        [JsonConstructor]
        public GetTestDetailsRequest(int countFromEnd, params IFilterOrder[] filteringOrders)
        {
            FilteringOrders = filteringOrders ?? throw new ArgumentNullException(nameof(filteringOrders));
            CountFromEnd = countFromEnd;
        }
    }
}
