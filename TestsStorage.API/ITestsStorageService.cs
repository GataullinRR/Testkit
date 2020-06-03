using System.Threading.Tasks;

namespace TestsStorageService.API
{
    public interface ITestsStorageService
    {
        Task<ListTestsDataResponse> ListTestsDataAsync(ListTestsDataRequest request);

        Task<DeleteTestResponse> DeleteTestAsync(DeleteTestRequest request);

        Task<SaveTestResponse> SaveTestAsync(SaveTestRequest request);
    }
}
