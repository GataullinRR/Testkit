using Grpc.Core;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestsStorageService.API;
using TestsStorageService.Db;
using Utilities.Types;
using Utilities.Extensions;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using Google.Protobuf;
using Shared;

namespace TestsStorageService
{
    [GrpcService]
    public class GrpcService : API.TestsStorageService.TestsStorageServiceBase
    {
        [Inject] public TestsContext Db { get; set; }
        
        public GrpcService(IDependencyResolver di)
        {
            di.ResolveProperties(this);
        }

        public override async Task<ListTestsDataResponse> ListTestsData(ListTestsDataRequest request, ServerCallContext context)
        {
            var count = ((int)request.ByRange.To - (int)request.ByRange.From)
                .NegativeToZero();
            var result = await Db.Cases
                .AsNoTracking()
                .OrderBy(e => e.Id)
                .Take(count)
                .ToArrayAsync();
            var serialized = JsonConvert.SerializeObject(result);

            return new ListTestsDataResponse()
            {
                Count = (uint)await Db.Cases.CountAsync(),
                Tests = ByteString.CopyFromUtf8(serialized),
                Status = new Protobuf.ResponseStatus()
            };
        }
    }
}
