using System.ComponentModel.DataAnnotations;
using TestsStorageService.API;

namespace TestsStorageService.API
{
    public class TestCase
    {
        [Required]
        public string Id { get; set; }
        
        [Required]
        public string AuthorName { get; set; }

        [Required]
        public CSTestCaseInfo CaseInfo { get; set; }
    }
}
