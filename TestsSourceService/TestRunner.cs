using MessageHub;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using RunnerService.APIModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities;
using Utilities.Types;
using Utilities.Extensions;

namespace ExampleTestsSourceService
{
    [Service(ServiceLifetime.Singleton, RegisterAsPolicy.Self)]
    public class TestRunner
    {
        [Inject] public IMessageConsumer Consumer { get; set; }
        [Inject] public IMessageProducer Producer { get; set; }

        public TestRunner(IDependencyResolver di)
        {
            di.ResolveProperties(this);

            Consumer.BeginTestAsync += Consumer_BeginTestAsync;
        }

        async Task Consumer_BeginTestAsync(BeginTestMessage arg)
        {
            var completedMessage = new TestCompletedOnSourceMessage(
                    arg.TestId,
                    arg.ResultId,
                    arg.TestId == "ER1"
                        ? new RunnerErrorResult()
                        {
                            StartTime = DateTime.UtcNow,
                            Duration = TimeSpan.FromSeconds(Global.Random.NextDouble(0, 10)),
                        }.To<RunResultBase>()
                        : new PassedResult()
                        {
                            StartTime = DateTime.UtcNow,
                            Duration = TimeSpan.FromSeconds(Global.Random.NextDouble(0, 10)),
                        });
            Producer.FireTestCompletedOnSource(completedMessage);
        }
    }
}
