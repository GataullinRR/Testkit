namespace PresentationService.API
{
    public class StopAddTestRequest
    {
        public static implicit operator global::PresentationService.API2.GStopAddTestRequest(StopAddTestRequest request)
        {
            var gRequest = new API2.GStopAddTestRequest()
            {
                 
            };

            return gRequest;
        }
        public static implicit operator StopAddTestRequest(global::PresentationService.API2.GStopAddTestRequest request)
        {
            return new StopAddTestRequest();
        }

        public StopAddTestRequest()
        {

        }
    }
}
