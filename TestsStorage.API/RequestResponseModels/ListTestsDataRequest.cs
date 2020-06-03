using Newtonsoft.Json;
using Protobuf;
using Shared.Types;
using System.Linq;
using Utilities.Extensions;
using Vectors;

namespace TestsStorageService.API
{
    public class ListTestsDataRequest
    {
        public bool IsById { get; }
        public int[]? TestIds { get; }

        public bool IsByAuthorName { get; }
        public string? AuthorName { get; }

        public string[]? TestNameFilters { get; }

        [JsonConverter(typeof(IntIntervalSerializer))]
        public IntInterval Range { get; }
        public bool IncludeData { get; }
        public bool ReturnNotSaved { get; }
  
        public ListTestsDataRequest(string authorName, IntInterval range, bool includeData, bool returnNotSaved) 
            : this(false, null, true, authorName, null, range, includeData, returnNotSaved)
        {

        }
        public ListTestsDataRequest(int[] testIds, IntInterval range, bool includeData, bool returnNotSaved)
            : this(true, testIds, false, null, null, range, includeData, returnNotSaved)
        {

        }
        public ListTestsDataRequest(string[] testNameFilters, IntInterval range, bool includeData, bool returnNotSaved)
            : this(false, null, false, null, testNameFilters, range, includeData, returnNotSaved)
        {

        }

        [JsonConstructor]
        ListTestsDataRequest(bool isById, int[]? testIds, bool isByAuthorName, string? authorName, string[]? testNameFilters, IntInterval range, bool includeData, bool returnNotSaved)
        {
            IsById = isById;
            TestIds = testIds;
            IsByAuthorName = isByAuthorName;
            AuthorName = authorName;
            TestNameFilters = testNameFilters;
            Range = range;
            IncludeData = includeData;
            ReturnNotSaved = returnNotSaved;
        }
    }
}
