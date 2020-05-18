using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using System.Linq;
using System.Threading.Tasks;
using Utilities.Types;

namespace Runner
{
    [Service(ServiceLifetime.Singleton)]
    public class Browser
    {
        readonly IJSRuntime _runtime;

        public Browser(IJSRuntime runtime)
        {
            _runtime = runtime;
        }

        public async Task AlertAsync(string message)
        {
            await _runtime.InvokeAsync<object>("alert", new object[] { message });
        }

        public async Task LogToConsoleAsync(string message)
        {
            await _runtime.InvokeAsync<object>("console.log", new object[] { message });
        }
    }
}
