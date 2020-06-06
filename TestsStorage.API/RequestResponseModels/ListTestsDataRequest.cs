using Newtonsoft.Json;
using Shared.Types;
using System.Collections.Generic;
using System.Linq;
using Utilities.Extensions;
using Vectors;

namespace TestsStorageService.API
{
    public class ListTestsDataRequest
    {
        public bool IsByIds { get; }
        public int[]? TestIds { get; }

        public bool IsByAuthorName { get; }
        public string? AuthorName { get; }

        public bool IsByNameFilters { get; }
        public string[]? TestNameFilters { get; }

        public bool IsByParameters { get; }
        public Dictionary<string, string>? TestParameters { get; set; }
        
        [JsonConverter(typeof(IntIntervalSerializer))]
        public IntInterval Range { get; }
        public bool IncludeData { get; }
        public bool ReturnNotSaved { get; }

        [JsonConstructor]
        ListTestsDataRequest(
            bool isByIds, int[]? testIds,
            bool isByAuthorName, string? authorName,
            bool isByNameFilters, string[]? testNameFilters,
            bool isByParameters, Dictionary<string, string>? testParameters,
            IntInterval range, bool returnNotSaved, bool includeData)
        {
            IsByIds = isByIds;
            TestIds = testIds;
            IsByAuthorName = isByAuthorName;
            AuthorName = authorName;
            IsByNameFilters = isByNameFilters;
            TestNameFilters = testNameFilters;
            IsByParameters = isByParameters;
            TestParameters = testParameters;
            Range = range;
            ReturnNotSaved = returnNotSaved;
            IncludeData = includeData;
        }

        public static ListTestsDataRequest ByIds(int[] ids, IntInterval range, bool returnNotSaved, bool includeData)
        {
            return new ListTestsDataRequest(true, ids, false, default, false, default, false, default, range, returnNotSaved, includeData);
        }
        public static ListTestsDataRequest ByAuthorName(string? authorName, IntInterval range, bool returnNotSaved, bool includeData)
        {
            return new ListTestsDataRequest(false, default, true, authorName, false, default, false, default, range, returnNotSaved, includeData);
        }
        public static ListTestsDataRequest ByNameFilter(string[]? nameFilters, IntInterval range, bool returnNotSaved, bool includeData)
        {
            return new ListTestsDataRequest(false, default, false, default, true, nameFilters, false, default, range, returnNotSaved, includeData);
        }
        public static ListTestsDataRequest ByParameters(Dictionary<string, string> testParameters, IntInterval range, bool returnNotSaved, bool includeData)
        {
            return new ListTestsDataRequest(false, default, false, default, false, default, true, testParameters, range, returnNotSaved, includeData);
        }
    }
}
