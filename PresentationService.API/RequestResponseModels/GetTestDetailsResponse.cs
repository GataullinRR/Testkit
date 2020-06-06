using AutoMapper.Mappers;
using Newtonsoft.Json;
using RunnerService.API;
using RunnerService.API.Models;
using Shared.Types;
using System;
using System.ComponentModel.DataAnnotations;
using Utilities;

namespace PresentationService.API
{
    public class GetTestDetailsResponse
    {
        public static implicit operator GetTestDetailsResponse(global::RunnerService.API.Models.GetTestDetailsResponse response)
        {
            return new GetTestDetailsResponse(response.RunResulsts, response.TotalCount);
        }

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
