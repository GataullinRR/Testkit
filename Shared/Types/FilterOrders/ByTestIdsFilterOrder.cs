using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace SharedT.Types
{
    public class ByTestIdsFilter : IFilterOrder
    {
        [Required]
        public int[] TestIds { get; }
       
        [JsonConstructor]
        public ByTestIdsFilter(params int[] testIds)
        {
            TestIds = testIds ?? throw new ArgumentNullException(nameof(testIds));
        }
    }
}
