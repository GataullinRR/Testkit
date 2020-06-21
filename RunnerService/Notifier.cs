using MessageHub;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RunnerService.API;
using RunnerService.Db;
using System.Linq;
using System.Threading.Tasks;
using Utilities.Types;
using Utilities.Extensions;
using RunnerService.API.Models;
using Grpc.Core.Logging;
using Microsoft.Extensions.Logging;

namespace RunnerService
{
    [Service(ServiceLifetime.Singleton, RegisterAsPolicy.Self)]
    public class Notifier
    {
        [Inject] public IMessageConsumer MessageConsumer { get; set; }
        [Inject] public IMessageProducer MessageProducer{ get; set; }
        [Inject] public IServiceScopeFactory ScopeFactory { get; set; }
        [Inject] public ILogger<Notifier> Logger{ get; set; }

        public Notifier(IDependencyResolver di)
        {
            di.ResolveProperties(this);

            MessageConsumer.TestCompletedOnSourceAsync += MessageConsumer_TestCompletedOnSourceAsync;
            MessageConsumer.TestDeletedAsync += MessageConsumer_TestDeletedAsync;
            MessageConsumer.CancelTestAsync += MessageConsumer_CancelTestAsync;
            MessageConsumer.TestResultStateAcquiredAsync += MessageConsumer_TestResultStateAcquiredAsync;
        }

        async Task MessageConsumer_TestResultStateAcquiredAsync(TestResultStateAcquiredMessage arg)
        {
            using var scope = ScopeFactory.CreateScope();
            using var db = scope.ServiceProvider.GetRequiredService<RunnerContext>();

            var rr = db.RunResults
                .IncludeGroup(API.Models.EntityGroups.ALL, db)
                .FirstOrDefault(r => r.Id == arg.ResultId);
            if (rr != null)
            {
                rr.ResultBase.State = arg.NewState;
                await db.SaveChangesAsync();

                MessageProducer.FireTestResultStateUpdated(new TestResultStateUpdatedMessage(rr.ResultBase.TestId, arg.ResultId, arg.NewState));
            }
        }

        async Task MessageConsumer_CancelTestAsync(CancelTestMessage arg)
        {
            using var scope = ScopeFactory.CreateScope();
            using var db = scope.ServiceProvider.GetRequiredService<RunnerContext>();
            var testIds = await db.TestRuns
                .Filter(Logger, arg.FilteringOrders)
                .Select(r => r.TestId)
                .ToArrayAsync();
            var runs = await db.RunResults
                .Include(r => r.ResultBase)
                .Where(r => r.ResultBase.Result == RunResult.Running)
                .Where(r => testIds.Contains(r.ResultBase.TestId))
                .ToArrayAsync();
            if (runs.Length != 0)
            {
                foreach (var run in runs)
                {
                    run.ResultBase = new AbortedResult()
                    {
                        ActualParameters = run.ResultBase.ActualParameters,
                        ExpectedParameters = run.ResultBase.ExpectedParameters,
                        Duration = run.ResultBase.Duration,
                        StartedByUser = run.ResultBase.StartedByUser,
                        StartTime = run.ResultBase.StartTime,
                        TestId = run.ResultBase.TestId,
                        TestName = run.ResultBase.TestName,
                        State = new StateInfo(null, null, true)
                    };
                }
                await db.SaveChangesAsync();
                
                foreach (var run in runs)
                {
                    MessageProducer.FireTestCancelled(new TestCancelledMessage(run.ResultBase.TestId, run.ResultBase.Id, run.ResultBase.TestName, run.ResultBase.StartedByUser));
                }
            }
        }

        async Task MessageConsumer_TestDeletedAsync(TestDeletedMessage arg)
        {
            using var scope = ScopeFactory.CreateScope();
            var sp = scope.ServiceProvider;
            using var db = sp.GetRequiredService<RunnerContext>();

            var runInfo = await db.TestRuns
                .IncludeGroup(API.Models.EntityGroups.ALL, db)
                .FirstOrDefaultAsync(r => r.TestId == r.TestId || r.TestName == arg.TestName);
            if (runInfo != null)
            {
                db.TestRuns.Remove(runInfo);
                await db.SaveChangesAsync();
            }
        }

        async Task MessageConsumer_TestCompletedOnSourceAsync(TestCompletedOnSourceMessage arg)
        {
            using var scope = ScopeFactory.CreateScope();
            using var db = scope.ServiceProvider.GetRequiredService<RunnerContext>();

            var runInfo = await db.TestRuns
                .IncludeGroup(API.Models.EntityGroups.ALL, db)
                .FirstAsync(r => r.TestId == arg.TestId);
            var result = runInfo.Results
                .FirstOrDefault(r => r.Id == arg.ResultId);
            if (result != null) // because it could have been deleted
            {
                if (result.ResultBase.Result != RunResult.Aborted)
                {
                    arg.Result.TestId = arg.TestId;
                    arg.Result.TestName = runInfo.TestName;
                    arg.Result.StartedByUser = result.ResultBase.StartedByUser;
                    result.ResultBase = arg.Result;
                    await db.SaveChangesAsync();

                    MessageProducer.FireTestCompleted(new TestCompletedMessage()
                    {
                        TestId = runInfo.TestName,
                        Result = arg.Result
                    });
                }
            }
        }
    }
}
