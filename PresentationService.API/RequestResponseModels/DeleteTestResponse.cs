using Protobuf;
using Shared.Types;

namespace PresentationService.API
{
    public class DeleteTestResponse : ResponseBase
    {
        public static implicit operator GDeleteTestResponse(DeleteTestResponse response)
        {
            return new GDeleteTestResponse()
            {
                Status = response.Status
            };
        }
        public static implicit operator DeleteTestResponse(GDeleteTestResponse response)
        {
            return new DeleteTestResponse(response.Status);
        }

        public DeleteTestResponse(ResponseStatus status) : base(status)
        {

        }
    }
}
