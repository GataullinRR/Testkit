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
                },
                ReturnNotSaved = request.ReturnNotSaved
            };
            gRequest.TestNameFilters.Add(request.TestNameFilters);

            return gRequest;
        }
        public static implicit operator ListTestsDataRequest(GListTestsDataRequest request)
        {
            return new ListTestsDataRequest(request.TestNameFilters?.ToArray(), 
                new IntInterval(request.Range.From, request.Range.To), 
                request.IncludeData,
                request.ReturnNotSaved);
        }

        public int[] TestIds { get; }
        public string[] TestNameFilters { get; }

        public IntInterval Range { get; }
        public bool IncludeData { get; }
        public bool ReturnNotSaved { get; }
        public bool IsById { get; }

        public ListTestsDataRequest(int[] testIds, IntInterval range, bool includeData, bool returnNotSaved)
        {
            IsById = true;
            TestIds = testIds;
            TestNameFilters = new string[0];
            Range = range;
            IncludeData = includeData;
            ReturnNotSaved = returnNotSaved;
        }
        public ListTestsDataRequest(string[] testNameFilters, IntInterval range, bool includeData, bool returnNotSaved)
        {
            TestIds = new int[0];
            TestNameFilters = testNameFilters;
            Range = range;
            IncludeData = includeData;
            ReturnNotSaved = returnNotSaved;
        }
    }
}
