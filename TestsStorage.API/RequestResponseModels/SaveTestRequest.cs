using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;

namespace TestsStorageService.API
{
    public class SaveTestRequest
    {
        [Required]
        public int TestId { get; }
        
        [Required]
        public string Name { get; }

        [Required]
        public string Description { get; }

        [Required]
        public string AuthorName { get; }

        [JsonConstructor]
        public SaveTestRequest(int testId, string name, string description, string authorName)
        {
            TestId = testId;
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            AuthorName = authorName ?? throw new ArgumentNullException(nameof(authorName));
        }
    }
}
