﻿using Microsoft.AspNetCore.Components;
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

namespace RunnerService
{
    class StateCleaner : IHostedService
    {
        [Inject] public IServiceScopeFactory ScopeFactory { get; set; }

        public StateCleaner(IDependencyResolver di)
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

                    var hang = await db.RunResults
                        .IncludeGroup(EntityGroups.ALL, db)
                        .AsNoTracking()
                        .ToArrayAsync();
                    hang = hang
                        .Where(d => d.ResultBase.Result == API.Models.RunResult.PendingCompletion 
                            && DateTime.UtcNow - d.ResultBase.StartTime > TimeSpan.FromMinutes(3))
                        .ToArray();
                    db.RunResults.RemoveRange(hang);
                    await db.SaveChangesAsync();
                }
                catch (Exception ex)
                {
                    Debugger.Break();
                }

                await Task.Delay(60 * 1000);
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {

        }
    }
}
