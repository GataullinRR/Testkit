using Newtonsoft.Json;
using SharedT.Types;
using System;
using System.ComponentModel.DataAnnotations;

namespace TestsStorageService.API
{
    public class ByAuthorsFilter : IFilterOrder
    {
        [Required]
        public string[] AuthorNames { get; }
        
        [JsonConstructor]
        public ByAuthorsFilter(string[] authorNames)
        {
            AuthorNames = authorNames ?? throw new ArgumentNullException(nameof(authorNames));
        }
    }
}
