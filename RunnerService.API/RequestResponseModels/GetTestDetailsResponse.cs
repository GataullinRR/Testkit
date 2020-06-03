using Google.Protobuf;
using Newtonsoft.Json;
using RunnerService.API;
using Shared.Types;
using System;
using System.ComponentModel.DataAnnotations;

namespace RunnerService.API.Models
{
    public class GetTestDetailsResponse
    {
        /// <summary>
        /// From new to old
        /// </summary>
        [Required]
        public RunResultBase[] RunResulsts { get; }

        [Required]
        public int TotalCount { get; }

        [JsonConstructor]
        public GetTestDetailsResponse(RunResultBase[] runResulsts, int totalCount)
        {
            RunResulsts = runResulsts ?? throw new ArgumentNullException(nameof(runResulsts));
            TotalCount = totalCount;
        }
    }
}
