using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace StateService.API
{
    public class GetTestsAddStateResponse
    {
        [Required]
        public bool HasBegun { get; }

        [JsonConstructor]
        public GetTestsAddStateResponse(bool hasBegun)
        {
            HasBegun = hasBegun;
        }
    }
}
