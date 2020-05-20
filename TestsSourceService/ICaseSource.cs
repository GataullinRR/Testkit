using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TestsStorageService.API;

namespace ExampleTestsSourceService
{
    public interface ICaseSource
    {
        Task<CSTestCaseInfo> GetCaseAsync(IDictionary<string, string> filter, CancellationToken cancellation);
    }
}
