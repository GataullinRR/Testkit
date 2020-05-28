using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using TestsStorageService.API;
using Utilities.Types;
using Utilities.Extensions;
using Utilities;
using System.Text.Json.Serialization;
using DDD;

namespace ExampleTestsSourceService
{
    [Service(ServiceLifetime.Singleton)]
    class CaseSource :  ICaseSource
    {
        public async Task<TestCaseInfo?> GetCaseAsync(IDictionary<string, string> filter, CancellationToken cancellation)
        {
            foreach (var kvp in filter)
            {
                if (kvp.Key == "UserId")
                {
                    var testCase = kvp.Value switch
                    {
                        "EX1" => new TestCaseInfo()
                        {
                            Data = "Hello1".GetASCIIBytes(),
                            DisplayName = "EX1 Case",
                            TargetType = "UI.OrderForm",
                        },

                        "EX2" => new TestCaseInfo()
                        {
                            Data = "Hello2".GetASCIIBytes(),
                            DisplayName = "EX2 Case",
                            TargetType = "Microservice.Order",
                        },

                        _ => null
                    };
                    testCase = testCase ?? new TestCaseInfo()
                    {
                        DisplayName = "Case with user:" + kvp.Value,
                        Data = ("Case with user:" + kvp.Value).GetASCIIBytes(),
                        TargetType = "None"
                    };

                    if (testCase != null)
                    {
                        return testCase;
                    }
                }
            }

            return null;
        }
    }
}
