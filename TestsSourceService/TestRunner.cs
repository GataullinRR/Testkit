using MessageHub;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using RunnerService.API;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Utilities;
using Utilities.Types;
using Utilities.Extensions;
using System.Threading;
using System.Text;
using RunnerService.API.Models;

namespace ExampleTestsSourceService
{
    [Service(ServiceLifetime.Singleton, RegisterAsPolicy.Self)]
    public class TestRunner
    {
        readonly IDictionary<string, CancellationTokenSource> _addProcesses = new Dictionary<string, CancellationTokenSource>();

        [Inject] public IMessageConsumer Consumer { get; set; }
        [Inject] public IMessageProducer Producer { get; set; }
        [Inject] public ICaseSource CaseSource { get; set; }

        public TestRunner(IDependencyResolver di)
        {
            di.ResolveProperties(this);
            accquireDaemon();

            Consumer.BeginTestAsync += Consumer_BeginTestAsync;
        }

        async void accquireDaemon()
        {
            await ThreadingUtils.ContinueAtDedicatedThread();

            while (true)
            {
                var parameters = "<Operation>";
                for (int i = 0; i < Global.Random.Next(1, 4); i++)
                {
                    parameters += $"<Step{i}>";

                    for (int k = 0; k < Global.Random.Next(2, 10); k++)
                    {
                        parameters += $"<p name=\"{Global.Random.NextRUWord()}\">{Global.Random.NextObjFrom(Global.Random.NextDateTime(default, DateTime.UtcNow).ToString(), Global.Random.NextDouble(0, 10000).ToString(), Global.Random.Next(-10000, 10000).ToString(), Global.Random.NextRUWord().ToString(), Global.Random.NextENWord().ToString())}</p>";
                    }

                    parameters += $"</Step{i}>";
                }
                parameters += "</Operation>";

                Producer.FireTestAcquired(new TestAcquiringResultMessage(
                    Global.Random.NextENWord(),
                    Global.Random.NextRUWord(),
                    $"SomeType{Global.Random.Next(0, 3)}",
                    new byte[] { 0, 2, 4, 8 },
                    new Dictionary<string, string>()
                    {
                        { "ServiceId", Global.Random.Next(0, 5).ToString() },
                        { "UserId", Global.Random.Next(0, 5).ToString() },
                    }, parameters));

                await Task.Delay(10000);
            }
        }

        async Task Consumer_BeginTestAsync(BeginTestMessage arg)
        {
            await Task.Delay(3000);

            var completedMessage = new TestCompletedOnSourceMessage(
                    arg.TestId,
                    arg.ResultId,
                    new PassedResult()
                    {
                        StartTime = DateTime.UtcNow,
                        Duration = TimeSpan.FromSeconds(Global.Random.NextDouble(0, 10)),
                    });
            Producer.FireTestCompletedOnSource(completedMessage);
        }
    }
}
