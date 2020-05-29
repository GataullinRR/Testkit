using Protobuf;
using System;
using System.Linq;
using Vectors;

namespace PresentationService.API
{
    public class ListTestsRequest
    {
        public static implicit operator global::PresentationService.API2.GListTestsRequest(ListTestsRequest request)
        {
            var gRequest = new API2.GListTestsRequest()
            {
                Range = new GRange()
                {
                    From = request.Range.From,
                    To = request.Range.To,
                },
                ReturnNotSaved = request.ReturnNotSaved,
                IsByIds = request.IsByIds,
                IsByAuthorName = request.IsByAuthorName,
            };
            if (request.IsByIds)
            {
                gRequest.TestIds.Add(request.TestIds);
            }
            if (request.IsByAuthorName)
            {
                gRequest.AuthorName = request.AuthorName;
            }
            else
            {
                if (request.TestNameFilter != null)
                {
                    gRequest.TestName = request.TestNameFilter;
                }
            }

            return gRequest;
        }
        public static implicit operator ListTestsRequest(global::PresentationService.API2.GListTestsRequest request)
        {
            var range = new IntInterval(request.Range.From, request.Range.To);
            if (request.IsByIds)
            {
                return new ListTestsRequest(request.TestIds.ToArray(), request.ReturnNotSaved, range);
            }
            else if (request.IsByAuthorName)
            {
                return ByAuthorName(request.AuthorName, request.ReturnNotSaved, range);
            }   
            else
            {
                return new ListTestsRequest(request.TestName, request.ReturnNotSaved, range);
            }
        }

        public bool IsByIds { get; }
        public int[]? TestIds { get; }

        public bool IsByAuthorName { get; }
        public string? AuthorName { get; }

        public string? TestNameFilter { get; }
        public bool ReturnNotSaved { get; }
        public IntInterval Range { get; }

        public ListTestsRequest(string? testNameFilter, bool returnNotSaved, IntInterval range) 
            : this(false, null, false, null, testNameFilter, returnNotSaved, range)
        {

        }
        public ListTestsRequest(int[] testIds, bool returnNotSaved, IntInterval range) 
            : this(false, testIds, false, null, null, returnNotSaved, range)
        {

        }
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
