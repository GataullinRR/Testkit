using System.Threading.Tasks;
using Utilities.Types;
using Utilities.Extensions;
using TestsSourceService.API;
using Grpc.Core;
using Microsoft.AspNetCore.Components;
using Newtonsoft.Json;
using Google.Protobuf;
using MessageHub;
using Shared;
using RunnerService.APIModels;
using System;
using Utilities;

namespace ExampleTestsSourceService
{
    [GrpcService]
    public class GrpcService : TestsSourceService.API.TestsSourceService.TestsSourceServiceBase
    {
        public GrpcService(IDependencyResolver di)
        {
            di.ResolveProperties(this);
        }
    }
}
