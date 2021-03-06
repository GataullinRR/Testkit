﻿using Newtonsoft.Json;
using SharedT.Types;
using System;
using System.ComponentModel.DataAnnotations;

namespace PresentationService.API
{
    public class BeginTestRequest
    {
        [Required]
        public IFilterOrder[] FilteringOrders { get; }

        [JsonConstructor]
        public BeginTestRequest(params IFilterOrder[] filteringOrders)
        {
            FilteringOrders = filteringOrders ?? throw new ArgumentNullException(nameof(filteringOrders));
        }
    }
}
