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
            using var db = scope.ServiceProvider.GetRequiredService<TestsContext>();

            var test = new TestCase()
            {
                AuthorName = arg.AuthorName,
                CreationDate = DateTime.UtcNow,
                Data = new TestCaseData()
                {
                    Data = arg.TestData,
                    Type = arg.TestType,
                    Parameters = arg.Parameters
                },
                State = TestCaseState.RecordedButNotSaved,
            };
            await db.Cases.AddAsync(test);
            await db.SaveChangesAsync();

            MessageProducer.FireTestAdded(new TestAddedMessage(test.TestId, test.TestName, test.AuthorName));
        }
    }
}
