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
            var response = new ListTestsDataResponse()
            {
                Count = (uint)await Db.Cases.CountAsync(),
                Status = new Protobuf.ResponseStatus()
            };

            if (request.ByIds.Count > 0)
            {
                var ids = request.ByIds.ToArray();
                var result = await Db.Cases
                    .AsNoTracking()
                    .Where(c => ids.Contains(c.Id))
                    .ToArrayAsync();
                var serialized = JsonConvert.SerializeObject(result);

                response.Tests = ByteString.CopyFromUtf8(serialized);
            }
            else if (request.ByRange != null)
            {
                var count = ((int)request.ByRange.To - (int)request.ByRange.From)
                    .NegativeToZero();
                var result = await Db.Cases
                    .AsNoTracking()
                    .OrderBy(e => e.Id)
                    .Take(count)
                    .ToArrayAsync();
                var serialized = JsonConvert.SerializeObject(result);

                response.Tests = ByteString.CopyFromUtf8(serialized);
            }

            return response;
        }
    }
}
