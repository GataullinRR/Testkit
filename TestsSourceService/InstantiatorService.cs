using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using System.Threading;

namespace ExampleTestsSourceService
{
    class InstantiatorService : IHostedService
    {
        public InstantiatorService(TestRunner runner)
        {

        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {

        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {

        }
    }
}
