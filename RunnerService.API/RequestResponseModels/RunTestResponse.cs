using Google.Protobuf;
using Newtonsoft.Json;
using RunnerService.API;
using Shared.Types;
using System;
using System.ComponentModel.DataAnnotations;

namespace RunnerService.API
{
    public class RunTestResponse : ResponseBase
    {
        public static implicit operator GRunTestResponse(RunTestResponse response)
        {
            return new GRunTestResponse()
            {
                Status = response.Status
            };
        }
        public static implicit operator RunTestResponse(GRunTestResponse response)
        {
            return new RunTestResponse(response.Status);
        }

        public RunTestResponse(ResponseStatus status) : base(status)
        {

        }
    }
}
