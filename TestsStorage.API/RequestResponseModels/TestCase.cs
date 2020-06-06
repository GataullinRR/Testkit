using System;
using System.ComponentModel.DataAnnotations;
using Utilities.Types;

namespace TestsStorageService.API
{
    public class TestCase
    {
        [Key]
        public int TestId { get; set; }

        [Required]
        public TestCaseState State { get; set; }

        [Required]
        [Include(EntityGroups.ALL)]
        public TestCaseData Data { get; set; }

        [Required]
        public DateTime CreationDate { get; set; }
        public DateTime? SaveDate { get; set; }

        public string? AuthorName { get; set; }
        public string TestName { get; set; }
        public string TestDescription { get; set; }
    }
}
