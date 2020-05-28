using Shared.Types;

namespace PresentationService.API
{
    public class StopAddTestResponse : ResponseBase
    {
        public static implicit operator global::PresentationService.API2.GStopAddTestResponse(StopAddTestResponse response)
        {
            return new global::PresentationService.API2.GStopAddTestResponse()
            {
                Status = response.Status
            };
        }
        public static implicit operator StopAddTestResponse(global::PresentationService.API2.GStopAddTestResponse response)
        {
            return new StopAddTestResponse(response.Status);
        }

        public StopAddTestResponse(ResponseStatus status) : base(status)
        {

        }
    }
}
