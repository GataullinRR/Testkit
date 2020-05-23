using MessageHub;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Protobuf;
using RunnerService.APIModels;
using RunnerService.Db;
using System.Threading.Tasks;
using Utilities.Types;

namespace RunnerService
{
    [Service(ServiceLifetime.Singleton, RegisterAsPolicy.Self)]
    public class Notifier
    {
        [Inject] public IMessageConsumer MessageConsumer { get; set; }
        [Inject] public IMessageProducer MessageProducer{ get; set; }
        [Inject] public IServiceScopeFactory ScopeFactory { get; set; }

        public Notifier(IDependencyResolver di)
        {
            di.ResolveProperties(this);

            MessageConsumer.TestCompletedOnSourceAsync += MessageConsumer_TestCompletedOnSourceAsync;
        }

        async Task MessageConsumer_TestCompletedOnSourceAsync(TestCompletedOnSourceMessage arg)
        {
            using var scope = ScopeFactory.CreateScope();
            var sp = scope.ServiceProvider;
            var db = sp.GetRequiredService<RunnerContext>();

            var runInfo = await db.TestRuns
                .Include(r => r.LastRun)
                .Include(r => r.State)
                .Include(r => r.RunPlan)
                .FirstAsync(r => r.TestId == arg.TestId);
            runInfo.LastRun = arg.Result;
            runInfo.State = new ReadyState();
            await db.SaveChangesAsync();

            var runInfo2 = await db.TestRuns
                .Include(r => r.LastRun)
                .Include(r => r.State)
                .Include(r => r.RunPlan)
                .ToArrayAsync();

            MessageProducer.FireTestCompleted(new TestCompletedMessage() 
            { 
                TestId = runInfo.TestId, 
                Result = runInfo.LastRun
            });
        }
    }
}
