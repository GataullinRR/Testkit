using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Utilities.Types;
using Microsoft.AspNetCore.Components;

namespace Runner
{
    [Service(ServiceLifetime.Singleton, RegisterAsPolicy.SelfAndFirstLevelInterfaces)]
    public class ServiceInitializator : IAppInitializationAwaiter
    {
        Task _initializationTask;
        [Inject] public SingletonInitializationService InitializationService { get; set; }

        public ServiceInitializator(IDependencyResolver di)
        {
            di.ResolveProperties(this);
        }

        public async Task BeginInitializationAsync(IServiceProvider serviceProvider)
        {
            _initializationTask = InitializationService.InitializeAsync(serviceProvider);

            await AwaitInitializedAsync();
        }

        public Task AwaitInitializedAsync()
        {
            if (_initializationTask == null)
            {
                throw new InvalidOperationException("Initialization process wasn't started!");
            }
            else
            {
                return _initializationTask;
            }
        }
    }
}
