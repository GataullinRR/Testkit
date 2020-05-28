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
        [Inject] public ICaseSource CaseSource { get; set; }

        public TestRunner(IDependencyResolver di)
        {
            di.ResolveProperties(this);

            Consumer.BeginTestAsync += Consumer_BeginTestAsync;
            Consumer.BeginAddTestAsync += Consumer_BeginAddTestAsync;
        }

        async Task Consumer_BeginAddTestAsync(BeginAddTestMessage arg)
        {
            while (true)
            {
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
                        Parameters = "<users><user name=\"Bill Gates\"><company>Microsoft</company><age>48</age ></user><user name=\"Larry Page\"><company>Google</company><age>42</age></user><parameter name=\"Author\">Hello, kitty!</parameter></users>",
                    });
                }

                await Task.Delay(Global.Random.Next(3000, 10000));
            }
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
