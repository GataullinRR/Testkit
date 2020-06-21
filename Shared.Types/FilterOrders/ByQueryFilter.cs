using Newtonsoft.Json;
using SharedT.Types;
using System;
using System.ComponentModel.DataAnnotations;

namespace TestsStorageService.API
{
    public class ByQueryFilter : IFilterOrder
    {
        [Required]
        public string Query { get; }
       
        [JsonConstructor]
        public ByQueryFilter(string query)
        {
            Query = query ?? throw new ArgumentNullException(nameof(query));
        }
    }
}
