using Protobuf;
using Vectors;

namespace PresentationService.API
{
    public class ListTestsRequest
    {
        public static implicit operator global::PresentationService.API2.GListTestsRequest (ListTestsRequest request)
        {
            return new API2.GListTestsRequest()
            {
                Range = new GRange()
                {
                    From = request.Range.From,
                    To = request.Range.To,
                }
            };
        }
        public static implicit operator ListTestsRequest(global::PresentationService.API2.GListTestsRequest request)
        {
            return new ListTestsRequest(new IntInterval(request.Range.From, request.Range.To));
        }

        public IntInterval Range { get; }

        public ListTestsRequest(IntInterval range)
        {
            Range = range;
        }
    }
}
