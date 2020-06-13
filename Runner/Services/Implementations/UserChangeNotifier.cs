using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Utilities.Extensions;
using Utilities.Types;

namespace Runner
{
    [Service(ServiceLifetime.Singleton)]
    class UserChangeNotifier : IUserChangeNotifier
    {
        [Inject] public Browser Browser { get; set; }

        public event Func<Task> ProfileChangedAsync = () => Task.CompletedTask;
        public event Func<Task> AuthStateChangedAsync = () => Task.CompletedTask;
        public event Func<Task> IdentityChangedAsync = () => Task.CompletedTask;

        public UserChangeNotifier(IDependencyResolver di)
        {
            di.ResolveProperties(this);
        }

        public async Task FireAuthStateChangedAsync()
        {
            await Browser.LogToConsoleAsync("AuthStateChangedAsync");

            await AuthStateChangedAsync.InvokeAndWaitAsync();
        }

        public async Task FireProfileChangedAsync()
        {
            await Browser.LogToConsoleAsync("ProfileChangedAsync");

            await ProfileChangedAsync.InvokeAndWaitAsync();
        }

        public async Task FireIdentityChangedAsync()
        {
            await Browser.LogToConsoleAsync("IdentityChanged");

            await IdentityChangedAsync.InvokeAndWaitAsync();
        }
    }
}
