using Protobuf;
using System;
using Vectors;

namespace PresentationService.API
{
    public class ListTestsRequest
    {
        public static implicit operator global::PresentationService.API2.GListTestsRequest (ListTestsRequest request)
        {
            var gRequest = new API2.GListTestsRequest()
            {
                Range = new GRange()
                {
                    From = request.Range.From,
                    To = request.Range.To,
                }
            };
            if (request.TestIdFilter != null)
            {
                gRequest.TestId = request.TestIdFilter;
            }

            return gRequest;
        }
        public static implicit operator ListTestsRequest(global::PresentationService.API2.GListTestsRequest request)
        {
            return new ListTestsRequest(request.TestId, new IntInterval(request.Range.From, request.Range.To));
        }

        public string? TestIdFilter { get; }
        public IntInterval Range { get; }

        public ListTestsRequest(IntInterval range) : this(null, range)
        {

        }
        public ListTestsRequest(string? testIdFilter, IntInterval range)
        {
            Range = range;
            TestIdFilter = testIdFilter;
        }
    }
}
