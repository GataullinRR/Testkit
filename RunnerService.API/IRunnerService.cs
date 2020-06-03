using RunnerService.API.Models;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace RunnerService.API
{
    public interface IRunnerService
    {
        Task<RunTestResponse> RunTestAsync(RunTestRequest request);
        Task<GetTestsInfoResponse> GetTestsInfoAsync(GetTestsInfoRequest request);
        Task<GetTestDetailsResponse> GetTestDetailsAsync(GetTestDetailsRequest request);
    }
}
