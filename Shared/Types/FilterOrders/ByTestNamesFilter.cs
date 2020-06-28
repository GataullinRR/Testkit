using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Linq.Expressions;

namespace SharedT.Types
{
    public class ByTestNamesFilter : IFilterOrder
    {
        [Required]
        public string[] TestNameFilters { get; }
       
        [JsonConstructor]
        public ByTestNamesFilter(params string[] testNameFilters)
        {
            TestNameFilters = testNameFilters ?? throw new ArgumentNullException(nameof(testNameFilters));
        }

        //public IQueryable<T> Filter<T>(IQueryable<T> query, Expression<Func<T, string>> nameProvider)
        //{
        //    query.Select(v => new { 
        //        V = v,
        //        Name = nameProvider.
        //    });

        //}    
    }
}
