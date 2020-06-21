using Newtonsoft.Json;
using SharedT.Types;
using System;
using System.ComponentModel.DataAnnotations;

namespace MessageHub
{
    public class CancelTestMessage
    {
        [Required]
        public IFilterOrder[] FilteringOrders { get; }

        [JsonConstructor]
        public CancelTestMessage(params IFilterOrder[] filteringOrders)
        {
            FilteringOrders = filteringOrders ?? throw new ArgumentNullException(nameof(filteringOrders));
        }
    }
}
