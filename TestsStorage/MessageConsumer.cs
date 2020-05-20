using Grpc.Core.Logging;
using MessageHub;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestsStorageService.API;
using TestsStorageService.Db;
using Utilities;
using Utilities.Types;
using Utilities.Extensions;

namespace TestsStorageService
{
#warning make hosted service
    [Service(ServiceLifetime.Singleton)]
    public class MessageConsumer
    {
        [Inject] public IServiceScopeFactory ScopeFactory { get; set; }
        [Inject] public ILogger<MessageConsumer> Logger { get; set; }

        public MessageConsumer(IDependencyResolver di, IMessageConsumer consumer)
        {
            di.ResolveProperties(this);

            consumer.TestAcquiredAsync += Consumer_TestAcquiredAsync;
        }

        async Task Consumer_TestAcquiredAsync(TestAcquiredMessage arg)
        {
            Logger.LogTrace("Got test: {0}", arg.Test.DisplayName);

            var testCase = new TestCase() 
            { 
                AuthorName = "Not yet supported",
                Id = arg.Test.CaseSourceId,
                CaseInfo = arg.Test 
            };

            using var scope = ScopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<TestsContext>();
            await db.Cases.AddAsync(testCase);
            await db.SaveChangesAsync();
        }
    }
}
