using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace DDD
{
    public class TestCaseInfo
    {
        [Required]
        public string DisplayName { get; set; }

        [Required]
        public string TargetType { get; set; }

        [Required]
        public byte[] Data { get; set; }
    }
}
