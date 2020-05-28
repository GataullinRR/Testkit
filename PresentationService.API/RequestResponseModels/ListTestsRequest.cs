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
            };
            if (request.IsByIds)
            {
                gRequest.TestIds.Add(request.TestIds);
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
            return new ListTestsRequest(request.IsByIds, request.TestIds.ToArray(), request.TestName, request.ReturnNotSaved, new IntInterval(request.Range.From, request.Range.To));
        }

        public bool IsByIds { get; }
        public int[] TestIds { get; }
        public string? TestNameFilter { get; }
        public bool ReturnNotSaved { get; }
        public IntInterval Range { get; }

        public ListTestsRequest(IntInterval range) : this(false, range)
        {

        }
        public ListTestsRequest(bool returnNotSaved, IntInterval range) : this((string?)null, returnNotSaved, range)
        {

        }
        public ListTestsRequest(string? testNameFilter, bool returnNotSaved, IntInterval range) : this(false, new int[0], testNameFilter, returnNotSaved, range)
        {

        }
        public ListTestsRequest(int[] testIds, bool returnNotSaved, IntInterval range) : this(true, testIds, null, returnNotSaved, range)
        {

        }
        ListTestsRequest(bool isByIds, int[] testIds, string? testNameFilter, bool returnNotSaved, IntInterval range)
        {
            IsByIds = isByIds;
            TestIds = testIds;
            TestNameFilter = testNameFilter;
            ReturnNotSaved = returnNotSaved;
            Range = range;
        }
    }
}
