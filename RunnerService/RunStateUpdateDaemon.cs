using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RunnerService.Db;
using System.Linq;
using System.Threading.Tasks;
using Utilities.Extensions;
using Microsoft.Extensions.Hosting;
using System.Threading;
using Utilities;
using System;
using System.Diagnostics;
using Utilities.Types;
using MessageHub;
using TestsStorageService.API;

namespace RunnerService
{
    class RunStateUpdateDaemon : IHostedService
    {
        [Inject] public IServiceScopeFactory ScopeFactory { get; set; }
        [Inject] public IMessageProducer Producer { get; set; }
        [Inject] public ITestsStorageService Storage { get; set; }

        public RunStateUpdateDaemon(IDependencyResolver di)
        {
            di.ResolveProperties(this);
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            daemon();
        }

        async Task daemon()
        {
            await ThreadingUtils.ContinueAtDedicatedThread();

            while (true)
            {
                try
                {
                    using var scope = ScopeFactory.CreateScope();
                    var db = scope.ServiceProvider.GetRequiredService<RunnerContext>();

                    var toUpdate = await db.RunResults
                        .AsNoTracking()
                        .IncludeGroup(API.Models.EntityGroups.ALL, db)
                        .Where(r => !r.ResultBase.State.IsFinal && r.ResultBase.State.NextStateUpdate != null && r.ResultBase.State.NextStateUpdate.Value < DateTime.UtcNow)
                        .ToArrayAsync();
                    foreach (var r in toUpdate)
                    {
                        Producer.FireUpdateTestResultState(new UpdateTestResultStateMessage(r.ResultBase.TestId, r.Id, r.ResultBase.SourceId, r.ResultBase.State));
                    }
                }
                catch (Exception ex)
                {
                    Debugger.Break();
                }

                await Task.Delay(10 * 1000);
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            
        }
    }
}
