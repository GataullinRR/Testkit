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
using System.Threading;
using System.Text;

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

            Consumer.BeginTestAsync += Consumer_BeginTestAsync;
            Consumer.BeginAddTestAsync += Consumer_BeginAddTestAsync;
            Consumer.StopAddTestAsync += Consumer_StopAddTestAsync;
        }

        async Task Consumer_StopAddTestAsync(StopAddTestMessage arg)
        {
            if (_addProcesses.ContainsKey(arg.UserName))
            {
                _addProcesses[arg.UserName].Cancel();
                _addProcesses.Remove(arg.UserName);
            }

            Producer.FireTestAddProgressChanged(new TestAddProgressChangedMessage(arg.UserName, "STOPPED"));
        }

        async Task Consumer_BeginAddTestAsync(BeginAddTestMessage arg)
        {
            var cts = new CancellationTokenSource();
            _addProcesses[arg.UserName] = cts;
            var additionalParameters = arg.Parameters.Select(p => $"<p name=\"{p.Key}\">{p.Value}</p>").Aggregate("");

            await ThreadingUtils.ContinueAtDedicatedThread();

            var sb = new StringBuilder();

            while (true)
            {
                sb.AppendLine($"[{DateTime.UtcNow.ToString()}] Got some data from /start");
                Producer.FireTestAddProgressChanged(new TestAddProgressChangedMessage(arg.UserName, sb.ToString()));

                var testCase = await CaseSource.GetCaseAsync(arg.Parameters, default);
                if (testCase == null)
                {
                    Producer.FireTestAcquired(new TestAcquiringResultMessage()
                    {
                        ResultCode = AcquiringResult.TargetNotFound
                    });
                }
                else
                {
                    Producer.FireTestAcquired(new TestAcquiringResultMessage()
                    {
                        AuthorName = arg.UserName,
                        TestData = testCase.Data,
                        TestType = testCase.TargetType,
                        Parameters = $"<root>{additionalParameters}<user name=\"Bill Gates\"><company>Microsoft</company><age>48</age ></user><user name=\"Larry Page\"><company>Google</company><age>42</age></user><parameter name=\"Author\">Hello, kitty!</parameter></root>",
                    });
                }

                await Task.Delay(Global.Random.Next(3000, 10000), cts.Token);
            }
        }

        async Task Consumer_BeginTestAsync(BeginTestMessage arg)
        {
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
