using PresentationService.API2;
using Protobuf;

namespace PresentationService.API
{
    public class GetTestsAddStateRequest
    {
        public static implicit operator GGetTestsAddStateRequest(GetTestsAddStateRequest request)
        {
            var gRequest = new GGetTestsAddStateRequest()
            {

            };

            return gRequest;
        }
        public static implicit operator GetTestsAddStateRequest(GGetTestsAddStateRequest request)
        {
            return new GetTestsAddStateRequest();
        }

        public GetTestsAddStateRequest()
        {

        }
    }
}
