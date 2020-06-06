using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using TestsStorageService.API;

namespace PresentationService.API
{
    public interface IPresentationService
    {
        Task<ListTestsResponse> ListTestsAsync(ListTestsRequest request);
        Task<BeginTestResponse> BeginTestAsync(BeginTestRequest request);
        Task<GetTestDetailsResponse> GetTestDetailsAsync(GetTestDetailsRequest request);
        Task<DeleteTestResponse> DeleteTestAsync(DeleteTestRequest request);
        Task<SaveRecordedTestResponse> SaveRecordedTestAsync(SaveRecordedTestRequest request);
    }
}
