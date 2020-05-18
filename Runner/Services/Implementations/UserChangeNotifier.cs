using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Utilities.Extensions;
using Utilities.Types;

namespace Runner
{
    [Service(ServiceLifetime.Singleton)]
    class UserChangeNotifier : IUserChangeNotifier
    {
        public event Func<Task> ProfileChangedAsync = () => Task.CompletedTask;
        public event Func<Task> AuthStateChangedAsync = () => Task.CompletedTask;

        public async Task FireAuthStateChangedAsync()
        {
            await AuthStateChangedAsync.InvokeAndWaitAsync();
        }

        public async Task FireProfileChangedAsync()
        {
            await ProfileChangedAsync.InvokeAndWaitAsync();
        }
    }
}
