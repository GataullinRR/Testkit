using System;
using System.ComponentModel.DataAnnotations;

namespace TestsStorageService.API
{
    public class TestCase
    {
        [Key]
        public string TestId { get; set; }

        [Required]
        public string AuthorName { get; set; }

        [Required]
        public string DisplayName { get; set; }

        public DateTime CreationDate { get; set; }

        public TestCaseState State { get; set; }

        public TestCaseData Data { get; set; }
    }
}
