using System;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace TestsStorageService.API
{
    public class CSTestCaseInfo
    {
        [Required]
        public string CaseSourceId { get; set; }

        [Required]
        public string DisplayName { get; set; }

        [Required]
        public string TargetType { get; set; }

        [Required]
        public byte[] Data { get; set; }
    }
}
