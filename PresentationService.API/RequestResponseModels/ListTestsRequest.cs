using Newtonsoft.Json;
using Shared.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using Vectors;

namespace PresentationService.API
{
    public class ListTestsRequest
    {
        public bool IsByIds { get; }
        public int[]? TestIds { get; }

        public bool IsByAuthorName { get; }
        public string? AuthorName { get; }

        public bool IsByNameFilter { get; }
        public string? TestNameFilter { get; }

        public bool IsByParameters { get; }
        public Dictionary<string, string>? TestParameters { get; set; }

        [JsonConverter(typeof(IntIntervalSerializer))]
        public IntInterval Range { get; }
        public bool ReturnNotSaved { get; }

        [JsonConstructor]
        ListTestsRequest(
            bool isByIds, int[]? testIds, 
            bool isByAuthorName, string? authorName,
            bool isByNameFilter, string? testNameFilter, 
            bool isByParameters, Dictionary<string, string>? testParameters, 
            IntInterval range, bool returnNotSaved)
        {
            IsByIds = isByIds;
            TestIds = testIds;
            IsByAuthorName = isByAuthorName;
            AuthorName = authorName;
            IsByNameFilter = isByNameFilter;
            TestNameFilter = testNameFilter;
            IsByParameters = isByParameters;
            TestParameters = testParameters;
            Range = range;
            ReturnNotSaved = returnNotSaved;
        }

        public static ListTestsRequest ByIdsName(int[] ids, IntInterval range, bool returnNotSaved)
        {
            return new ListTestsRequest(true, ids, false, default, false, default, false, default, range, returnNotSaved);
        }
        public static ListTestsRequest ByAuthorName(string? authorName, IntInterval range, bool returnNotSaved)
        {
            return new ListTestsRequest(false, default, true, authorName, false, default, false, default, range, returnNotSaved);
        }
        public static ListTestsRequest ByNameFilter(string? nameFilter, IntInterval range, bool returnNotSaved)
        {
            return new ListTestsRequest(false, default, false, default, true, nameFilter, false, default, range, returnNotSaved);
        }
        public static ListTestsRequest ByParameters(Dictionary<string, string> testParameters, IntInterval range, bool returnNotSaved)
        {
            return new ListTestsRequest(false, default, false, default, false, default, true, testParameters, range, returnNotSaved);
        }
    }
}
