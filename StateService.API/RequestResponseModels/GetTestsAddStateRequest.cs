using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;
using System.Text;

namespace StateService.API
{
    public class GetTestsAddStateRequest
    {
        [Required]
        public string UserName { get; }

        [JsonConstructor]
        public GetTestsAddStateRequest(string userName)
        {
            UserName = userName ?? throw new ArgumentNullException(nameof(userName));
        }
    }
}
