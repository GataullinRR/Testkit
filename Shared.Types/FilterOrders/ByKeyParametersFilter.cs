using Newtonsoft.Json;
using SharedT.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace TestsStorageService.API
{
    public class ByKeyParametersFilter : IFilterOrder
    {
        [Required]
        public Dictionary<string, string> TestParameters { get; }
        
        [JsonConstructor]
        public ByKeyParametersFilter(Dictionary<string, string> testParameters)
        {
            TestParameters = testParameters ?? throw new ArgumentNullException(nameof(testParameters));
        }
    }
}
