using Newtonsoft.Json;
using SharedT.Types;
using System;
using System.ComponentModel.DataAnnotations;
using Utilities.Extensions;

namespace TestsStorageService.API
{
    public class DeleteTestRequest
    {
        [Required]
        public IFilterOrder[] FilteringOrders { get; }

        public DeleteTestRequest(params IFilterOrder[] filteringorders)
        {
            FilteringOrders = filteringorders ?? throw new ArgumentNullException(nameof(filteringorders));
        }
    }
}
