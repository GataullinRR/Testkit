using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace TestsStorageService.API
{
    public class KeyParameter
    {
        [Required]
        public string Key { get; set; }
        
        [Required]
        public string Value { get; set; }

        KeyParameter() { } // For EF

        [JsonConstructor]
        public KeyParameter(string key, string value)
        {
            Key = key ?? throw new ArgumentNullException(nameof(key));
            Value = value ?? throw new ArgumentNullException(nameof(value));
        }
    }
}
