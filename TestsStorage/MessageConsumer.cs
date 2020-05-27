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
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;

namespace TestsStorageService
{
#warning make hosted service
    [Service(ServiceLifetime.Singleton)]
    public class MessageConsumer
    {
        [Inject] public IServiceScopeFactory ScopeFactory { get; set; }
        [Inject] public ILogger<MessageConsumer> Logger { get; set; }
        [Inject] public IMessageProducer MessageProducer { get; set; }

        public MessageConsumer(IDependencyResolver di, IMessageConsumer consumer)
        {
            di.ResolveProperties(this);

            consumer.TestAcquiredAsync += Consumer_TestAcquiredAsync;
        }

        async Task Consumer_TestAcquiredAsync(TestAcquiringResultMessage arg)
        {
            using var scope = ScopeFactory.CreateScope();
            var db = scope.ServiceProvider.GetRequiredService<TestsContext>();
            var test = await db.Cases.FirstAsync(c => c.TestId == arg.TestId && c.State == TestCaseState.NotRecorded);
            test.State = TestCaseState.Recorder;
            test.Data = new TestCaseData()
            {
                Type = arg.TestType,
                Data = arg.TestData,
                Parameters = arg.Parameters
            };
            await db.SaveChangesAsync();

            MessageProducer.FireTestRecorded(new TestRecordedMessage()
            {
                TestId = arg.TestId
            });
        }
    }
}
