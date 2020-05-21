using MessageHub;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RunnerService.APIModels;
using RunnerService.Db;
using System.Threading.Tasks;
using Utilities.Types;

namespace RunnerService
{
    [Service(ServiceLifetime.Singleton)]
    public class Notifier
    {
        [Inject] public IMessageConsumer MessageConsumer { get; set; }
        [Inject] public IMessageProducer MessageProducer{ get; set; }
        [Inject] public IServiceScopeFactory ScopeFactory { get; set; }

        public Notifier(IDependencyResolver di, IMessageConsumer messageConsumer)
        {
            di.ResolveProperties(this);

            messageConsumer.TestCompletedOnSourceAsync += MessageConsumer_TestCompletedOnSourceAsync;
        }

        async Task MessageConsumer_TestCompletedOnSourceAsync(TestCompletedOnSourceMessage arg)
        {
            using var scope = ScopeFactory.CreateScope();
            var sp = scope.ServiceProvider;
            var db = sp.GetRequiredService<RunnerContext>();

            var runInfo = await db.TestRuns
                .Include(r => r.LastRun)
                .Include(r => r.State)
                .FirstAsync(r => r.TestSourceId == arg.TestSourceId);
            runInfo.LastRun = arg.Result;
            runInfo.State = new ReadyState();

            await db.SaveChangesAsync();

            MessageProducer.FireTestCompleted(new TestCompletedMessage() 
            { 
                TestId = runInfo.TestId, 
                Result = runInfo.LastRun 
            });
        }
    }
}
