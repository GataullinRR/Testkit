using Google.Protobuf;
using Newtonsoft.Json;
using RunnerService.API;
using Shared.Types;
using System;
using System.ComponentModel.DataAnnotations;

namespace RunnerService.API
{
    public class GetTestsInfoResponse : ResponseBase
    {
        static readonly JsonSerializerSettings JSON_SETTING = new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.All
        };

        public static implicit operator GGetTestsInfoResponse(GetTestsInfoResponse response)
        {
            return new GGetTestsInfoResponse()
            {
                Infos = ByteString.CopyFromUtf8(JsonConvert.SerializeObject(response.RunInfos, JSON_SETTING)),
                Status = response.Status
            };
        }
        public static implicit operator GetTestsInfoResponse(GGetTestsInfoResponse response)
        {
            return new GetTestsInfoResponse(
                JsonConvert.DeserializeObject<TestRunInfo[]>(response.Infos.ToStringUtf8(), JSON_SETTING),
                response.Status);
        }

        [Required]
        public TestRunInfo[] RunInfos { get; set; }

        public GetTestsInfoResponse(TestRunInfo[] runInfos, ResponseStatus status) : base(status)
        {
            RunInfos = runInfos ?? throw new ArgumentNullException(nameof(runInfos));
        }
    }
}
