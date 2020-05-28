using Shared.Types;

namespace PresentationService.API
{
    public class SaveRecordedTestResponse : ResponseBase
    {
        public static implicit operator global::PresentationService.API2.GSaveRecordedTestResponse(SaveRecordedTestResponse response)
        {
            return new global::PresentationService.API2.GSaveRecordedTestResponse()
            {
                Status = response.Status
            };
        }
        public static implicit operator SaveRecordedTestResponse(global::PresentationService.API2.GSaveRecordedTestResponse response)
        {
            return new SaveRecordedTestResponse(response.Status);
        }

        public SaveRecordedTestResponse(ResponseStatus status) : base(status)
        {

        }
    }
}
