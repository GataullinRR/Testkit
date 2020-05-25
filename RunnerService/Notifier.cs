using MessageHub;
using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Protobuf;
using RunnerService.APIModels;
using RunnerService.Db;
using System.Linq;
using System.Threading.Tasks;
using Utilities.Types;
using Utilities.Extensions;

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
            MessageConsumer.TestDeletedMessageAsync += MessageConsumer_TestDeletedAsync; 
        }

        async Task MessageConsumer_TestDeletedAsync(TestDeletedMessage arg)
        {
            using var scope = ScopeFactory.CreateScope();
            var sp = scope.ServiceProvider;
            using var db = sp.GetRequiredService<RunnerContext>();

            var runInfo = await db.TestRuns.FirstOrDefaultAsync(r => r.TestId == arg.TestId);
            db.TestRuns.Remove(runInfo);
        }

        async Task MessageConsumer_TestCompletedOnSourceAsync(TestCompletedOnSourceMessage arg)
        {
            using var scope = ScopeFactory.CreateScope();
            var sp = scope.ServiceProvider;
            using var db = sp.GetRequiredService<RunnerContext>();

            var runInfo = await db.TestRuns
                .IncludeGroup(EntityGroups.ALL, db)
                .FirstAsync(r => r.TestId == arg.TestId);
            var result = runInfo.Results
                .FirstOrDefault(r => r.Id == arg.ResultId);
#warning MM
            arg.Result.StartedByUser = result.ResultBase.StartedByUser;
            result.ResultBase = arg.Result;
            runInfo.State = new ReadyState();
            await db.SaveChangesAsync();

            MessageProducer.FireTestCompleted(new TestCompletedMessage() 
            { 
                TestId = runInfo.TestId, 
                Result = arg.Result
            });
        }
    }
}
