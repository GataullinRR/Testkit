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
                ReturnNotSaved = request.ReturnNotSaved,
                IsByAuthorName = request.IsByAuthorName,
                IsById = request.IsById,
            };
            if (request.IsById)
            {
                gRequest.TestIds.Add(request.TestIds);
            }
            else if (request.IsByAuthorName)
            {
                gRequest.AuthorName = request.AuthorName;
            }
            else
            {
                gRequest.TestNameFilters.Add(request.TestNameFilters);
            }

            return gRequest;
        }
        public static implicit operator ListTestsDataRequest(GListTestsDataRequest request)
        {
            if (request.IsById)
            {
                return new ListTestsDataRequest(request.TestIds.ToArray(), new IntInterval(request.Range.From, request.Range.To), request.IncludeData, request.ReturnNotSaved);
            }
            else if (request.IsByAuthorName)
            {
                return new ListTestsDataRequest(request.AuthorName, new IntInterval(request.Range.From, request.Range.To), request.IncludeData, request.ReturnNotSaved);
            }
            else
            {
                return new ListTestsDataRequest(request.TestNameFilters.ToArray(), new IntInterval(request.Range.From, request.Range.To), request.IncludeData, request.ReturnNotSaved);
            }
        }

        public bool IsById { get; }
        public int[] TestIds { get; }

        public bool IsByAuthorName { get; }
        public string AuthorName { get; }

        public string[] TestNameFilters { get; }

        public IntInterval Range { get; }
        public bool IncludeData { get; }
        public bool ReturnNotSaved { get; }
  
        public ListTestsDataRequest(string authorName, IntInterval range, bool includeData, bool returnNotSaved) 
            : this(null, false, authorName, true, null, range, includeData, returnNotSaved)
        {

        }
        public ListTestsDataRequest(int[] testIds, IntInterval range, bool includeData, bool returnNotSaved)
            : this(testIds, true, null, false, null, range, includeData, returnNotSaved)
        {

        }
        public ListTestsDataRequest(string[] testNameFilters, IntInterval range, bool includeData, bool returnNotSaved)
            : this(null, false, null, false, testNameFilters, range, includeData, returnNotSaved)
        {

        }
        ListTestsDataRequest(int[]? testIds, bool isById, string? authorName, bool isByAuthorName, string[]? testNameFilters, IntInterval range, bool includeData, bool returnNotSaved)
        {
            TestIds = testIds;
            TestNameFilters = testNameFilters;
            Range = range;
            IncludeData = includeData;
            ReturnNotSaved = returnNotSaved;
            IsById = isById;
            IsByAuthorName = isByAuthorName;
            AuthorName = authorName;
        }
    }
}
