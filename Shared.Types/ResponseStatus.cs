using Protobuf;

namespace Shared.Types
{
    public class ResponseStatus
    {
        public static implicit operator ResponseStatus(Protobuf.StatusCode statusCode)
        {
            return new ResponseStatus(statusCode);
        }

        public static implicit operator Protobuf.GResponseStatus(ResponseStatus response)
        {
            var gResponse = new GResponseStatus()
            {
                Code = response.Code,
            };

            if (response.Description != null)
            {
                gResponse.Description = response.Description;
            }

            return gResponse;
        }
        public static implicit operator ResponseStatus(Protobuf.GResponseStatus response)
        {
            return new ResponseStatus(response.Code, response.Description);
        }
  
        public Protobuf.StatusCode Code { get; }
        public string? Description { get; }

        public ResponseStatus() : this(Protobuf.StatusCode.Ok, null) { }
        public ResponseStatus(Protobuf.StatusCode code) : this(code, null) { }
        public ResponseStatus(Protobuf.StatusCode code, string? description)
        {
            Code = code;
            Description = description;
        }
    }
}
