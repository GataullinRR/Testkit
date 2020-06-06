using System;
using System.Threading.Tasks;

namespace StateService.API
{
    public interface IStateService
    {
        Task<GetTestsAddStateResponse> GetTestsAddStateAsync(GetTestsAddStateRequest request);
    }
}
