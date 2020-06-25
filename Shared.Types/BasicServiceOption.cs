using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace SharedT.Types
{
    public class BasicServiceOption
    {
        [Required]
        public Uri Address { get; set; }

        [Required]
        public TimeSpan Timeout { get; set; } = TimeSpan.FromSeconds(10);
    }
}
