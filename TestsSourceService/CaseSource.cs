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

namespace ExampleTestsSourceService
{
    [Service(ServiceLifetime.Scoped)]
    class CaseSource :  ICaseSource
    {
        public async Task<CSTestCaseInfo?> GetCaseAsync(IDictionary<string, string> filter, CancellationToken cancellation)
        {
            foreach (var kvp in filter)
            {
                if (kvp.Key == "UserId")
                {
                    var testCase = kvp.Value switch
                    {
                        "EX1" => new CSTestCaseInfo()
                        {
                            CaseSourceId = "La1",
                            Data = "Hello1".GetASCIIBytes(),
                            DisplayName = "EX1 Case",
                            TargetType = "UI.OrderForm",
                        },

                        "EX2" => new CSTestCaseInfo()
                        {
                            CaseSourceId = "La2",
                            Data = "Hello2".GetASCIIBytes(),
                            DisplayName = "EX2 Case",
                            TargetType = "Microservice.Order",
                        },

                        _ => null
                    };
                    testCase = testCase ?? new CSTestCaseInfo()
                    {
                        CaseSourceId = "" + kvp.Value,
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
