using Google.Protobuf;
using Newtonsoft.Json;
using RunnerService.APIModels;
using Shared.Types;
using System;
using System.ComponentModel.DataAnnotations;

namespace PresentationService.API
{
    public class GetTestDetailsResponse : ResponseBase
    {
        static readonly JsonSerializerSettings JSON_SETTING = new JsonSerializerSettings()
        {
            TypeNameHandling = TypeNameHandling.All
        };

        public static implicit operator global::Protobuf.GGetTestDetailsResponse(GetTestDetailsResponse response)
        {
            var gResponse = new Protobuf.GGetTestDetailsResponse()
            {
                Status = response.Status,
                TotalCount = response.TotalCount,
                Details = ByteString.CopyFromUtf8(JsonConvert.SerializeObject(response.RunResulsts, JSON_SETTING))
            };
            gResponse.Details = ByteString.CopyFromUtf8(JsonConvert.SerializeObject(response.RunResulsts, JSON_SETTING));

            return gResponse;
        }
        public static implicit operator GetTestDetailsResponse(global::Protobuf.GGetTestDetailsResponse response)
        {
            return new GetTestDetailsResponse(
                JsonConvert.DeserializeObject<RunResultBase[]>(response.Details.ToStringUtf8(), JSON_SETTING),
                response.TotalCount,
                response.Status);
        }

        /// <summary>
        /// From new to old
        /// </summary>
        [Required]
        public RunResultBase[] RunResulsts { get; }

        public int TotalCount { get; }

        public GetTestDetailsResponse(RunResultBase[] runResulsts, int totalCount, ResponseStatus status) : base(status)
        {
            RunResulsts = runResulsts ?? throw new ArgumentNullException(nameof(runResulsts));
            TotalCount = totalCount;
        }
    }
}
