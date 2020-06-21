using Newtonsoft.Json;
using RunnerService.API;
using SharedT.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace RunnerService.API.Models
{
    public class RunTestRequest
    {
        [Required]
        public string StartedByUser { get; }

        [Required]
        public IFilterOrder[] FilteringOrders { get; }

        public RunTestRequest(string startedByUser, params IFilterOrder[] filteringOrders)
        {
            StartedByUser = startedByUser ?? throw new ArgumentNullException(nameof(startedByUser));
            FilteringOrders = filteringOrders ?? throw new ArgumentNullException(nameof(filteringOrders));
        }
    }
}
