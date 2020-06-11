using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using TestsStorageService.API;
using TestsStorageService.Db;
using Utilities;
using Utilities.Types;

namespace UserService
{
    class UnsavedTestsCleanerDaemon : IHostedService
    {
        [Inject] public IServiceScopeFactory ScopeFactory { get; set; }
        [Inject] public ILogger<UnsavedTestsCleanerDaemon> Logger { get; set; }

        public UnsavedTestsCleanerDaemon(IDependencyResolver di)
        {
            di.ResolveProperties(this);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            daemon();
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {

        }

        async Task daemon()
        {
            await ThreadingUtils.ContinueAtDedicatedThread();

            while (true)
            {
                try
                {
                    using var scope = ScopeFactory.CreateScope();
                    using var db = scope.ServiceProvider.GetRequiredService<TestsContext>();
                    var threshold = DateTime.UtcNow.AddDays(-3); 
                    var legacy = await db.Cases
                        .Where(c => c.State == TestCaseState.RecordedButNotSaved && c.CreationDate < threshold)
                        .ToArrayAsync();
                    foreach (var e in legacy)
                    {
                        e.IsDeleted = true;
                    }
                    await db.SaveChangesAsync();

                    Logger.LogInformation($"{legacy.Length} cases have been deleted");
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex, "Could not cleanup");
                }

                await Task.Delay(3600 * 1000);
            }
        }
    }
}
