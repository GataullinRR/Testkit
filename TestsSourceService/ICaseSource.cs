using DDD;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TestsStorageService.API;

namespace ExampleTestsSourceService
{
    public interface ICaseSource
    {
        Task<TestCaseInfo> GetCaseAsync(IDictionary<string, string> filter, CancellationToken cancellation);
    }
}
