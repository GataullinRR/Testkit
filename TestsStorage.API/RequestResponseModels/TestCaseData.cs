using System.ComponentModel.DataAnnotations;

namespace TestsStorageService.API
{
    public class TestCaseData
    {
        [Required]
        public string Type { get; set; }

        [Required]
        public byte[] Data { get; set; }
    }
}
