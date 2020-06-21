using Newtonsoft.Json;
using SharedT.Types;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Vectors;

namespace RunnerService.API.Models
{
    public class GetTestDetailsRequest
    {
        [Required]
        public IFilterOrder[] FilteringOrders { get; }

        [Required]
        public int CountFromEnd { get; }

        public GetTestDetailsRequest(int countFromEnd, params IFilterOrder[] filteringOrders)
        {
            FilteringOrders = filteringOrders ?? throw new ArgumentNullException(nameof(filteringOrders));
            CountFromEnd = countFromEnd;
        }
    }
}
