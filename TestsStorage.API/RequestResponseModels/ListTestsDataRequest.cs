using Protobuf;
using System;
using System.Linq;
using Utilities.Extensions;
using Vectors;

namespace TestsStorageService.API
{
    public class ListTestsDataRequest
    {
        public static implicit operator GListTestsDataRequest(ListTestsDataRequest request)
        {
            var gRequest = new GListTestsDataRequest()
            {
                IncludeData = request.IncludeData,
                Range = new GRange()
                {
                    From = request.Range.From,
                    To = request.Range.To,
                }
            };
            gRequest.TestIds.Add(request.TestIdsFilter);

            return gRequest;
        }
        public static implicit operator ListTestsDataRequest(GListTestsDataRequest request)
        {
            return new ListTestsDataRequest(request.TestIds?.ToArray(), 
                new IntInterval(request.Range.From, request.Range.To), 
                request.IncludeData);
        }

        public string[] TestIdsFilter { get; }
        public IntInterval Range { get; }
        public bool IncludeData { get; }

        public ListTestsDataRequest(string[] testIdsFilter, IntInterval range, bool includeData)
        {
            TestIdsFilter = testIdsFilter;
            Range = range;
            IncludeData = includeData;
        }
    }
}
