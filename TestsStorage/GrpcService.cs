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
using Protobuf;
using MessageHub;
using PresentationService.API;
using Shared.Types;

namespace TestsStorageService
{
    [GrpcService]
    public class GrpcService : API.TestsStorageService.TestsStorageServiceBase
    {
        [Inject] public IMessageProducer MessageProducer { get; set; }
        [Inject] public TestsContext Db { get; set; }
        
        public GrpcService(IDependencyResolver di)
        {
            di.ResolveProperties(this);
        }

        public override async Task<GListTestsDataResponse> ListTestsData(GListTestsDataRequest request, ServerCallContext context)
        {
            var response = new GListTestsDataResponse()
            {
                Count = (uint)await Db.Cases.CountAsync(),
                Status = new Protobuf.GResponseStatus()
            };

            var cases = Db.Cases
                .AsNoTracking()
                .OrderByDescending(c => c.CreationDate);
            if (request.ByIds.Count > 0)
            {
                var ids = request.ByIds.ToArray();
                var result = await cases
                    .Where(c => ids.Contains(c.TestId))
                    .ToArrayAsync();
                var serialized = JsonConvert.SerializeObject(result);

                response.Tests = ByteString.CopyFromUtf8(serialized);
            }
            else if (request.ByRange != null)
            {
                var count = ((int)request.ByRange.To - (int)request.ByRange.From)
                    .NegativeToZero();
                var result = await cases
                    .Take(count)
                    .ToArrayAsync();
                var serialized = JsonConvert.SerializeObject(result);

                response.Tests = ByteString.CopyFromUtf8(serialized);
            }

            return response;
        }

        public override async Task<GTryCreateTestResponse> TryCreateTest(GTryCreateTestRequest request, ServerCallContext context)
        {
            var response = new GTryCreateTestResponse()
            {
                Status = new Protobuf.GResponseStatus()
            };

            bool exists = await Db.Cases.AnyAsync(c => c.TestId == request.TestId);
            if (exists)
            {
                response.IsAlreadyAdded = true;
                response.Status.Code = Protobuf.StatusCode.Error;
            }
            else
            {
                await Db.Cases.AddAsync(new Db.TestCase()
                {
                    TestId = request.TestId,
                    AuthorName = request.Author,
                    DisplayName = request.DisplayName,
                    State = TestCaseState.NotRecorded,
                    CreationDate = DateTime.UtcNow,
                });
                await Db.SaveChangesAsync();
            }

            return response;
        }

        public override async Task<GDeleteTestResponse> DeleteTest(GDeleteTestRequest gRequest, ServerCallContext context)
        {
            DeleteTestRequest request = gRequest;

            var test = await Db.Cases.FirstOrDefaultAsync(c => c.TestId == request.TestId);
            if (test!= null)
            {
                Db.Cases.Remove(test);
            }
            await Db.SaveChangesAsync();

            MessageProducer.FireTestDeleted(new TestDeletedMessage(request.TestId));

            return new DeleteTestResponse(Protobuf.StatusCode.Ok);
        }
    }
}
