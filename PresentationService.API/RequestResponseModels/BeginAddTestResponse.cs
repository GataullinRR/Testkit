using Shared.Types;

namespace PresentationService.API
{
    public class BeginAddTestResponse : ResponseBase
    {
        public static implicit operator global::PresentationService.API2.GBeginAddTestResponse(BeginAddTestResponse response)
        {
            return new global::PresentationService.API2.GBeginAddTestResponse()
            {
                Status = response.Status
            };
        }
        public static implicit operator BeginAddTestResponse(global::PresentationService.API2.GBeginAddTestResponse response)
        {
            return new BeginAddTestResponse(response.Status);
        }

        public BeginAddTestResponse(ResponseStatus status) : base(status)
        {

        }
    }
}
