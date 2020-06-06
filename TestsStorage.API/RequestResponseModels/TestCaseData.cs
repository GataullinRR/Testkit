using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Utilities.Types;

namespace TestsStorageService.API
{
    public class TestCaseData
    {
        public string? Type { get; set; }
        public byte[]? Data { get; set; }

        /// <summary>
        /// Xml tree
        /// </summary>
        public string? Parameters { get; set; }

        [Required]
        [Include(EntityGroups.ALL)]
        public List<KeyParameter> KeyParameters { get; set; }
    }
}
