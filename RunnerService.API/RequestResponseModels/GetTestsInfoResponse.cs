﻿using Google.Protobuf;
using Newtonsoft.Json;
using RunnerService.API;
using Shared.Types;
using System;
using System.ComponentModel.DataAnnotations;

namespace RunnerService.API.Models
{
    public class GetTestsInfoResponse
    {
        [Required]
        public TestRunInfo[] RunInfos { get; }

        public GetTestsInfoResponse(TestRunInfo[] runInfos)
        {
            RunInfos = runInfos ?? throw new ArgumentNullException(nameof(runInfos));
        }
    }
}
