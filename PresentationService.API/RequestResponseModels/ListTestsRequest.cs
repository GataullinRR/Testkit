using Newtonsoft.Json;
using Protobuf;
using Shared.Types;
using System;
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

        public string? TestNameFilter { get; }
        public bool ReturnNotSaved { get; }
        
        [JsonConverter(typeof(IntIntervalSerializer))]
        public IntInterval Range { get; }

        public ListTestsRequest(string? testNameFilter, bool returnNotSaved, IntInterval range) 
            : this(false, null, false, null, testNameFilter, returnNotSaved, range)
        {

        }
        public ListTestsRequest(int[] testIds, bool returnNotSaved, IntInterval range) 
            : this(false, testIds, false, null, null, returnNotSaved, range)
        {

        }

        [JsonConstructor]
        ListTestsRequest(bool isByIds, int[]? testIds, bool isByAuthorName, string? authorName, string? testNameFilter, bool returnNotSaved, IntInterval range)
        {
            IsByIds = isByIds;
            TestIds = testIds;
            IsByAuthorName = isByAuthorName;
            AuthorName = authorName;
            TestNameFilter = testNameFilter;
            ReturnNotSaved = returnNotSaved;
            Range = range;
        }

        public static ListTestsRequest ByAuthorName(string? authorName, bool returnNotSaved, IntInterval range)
        {
            return new ListTestsRequest(false, null, true, authorName, null, returnNotSaved, range);
        }
    }
}
