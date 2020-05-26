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
using System.Linq.Expressions;

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

        public override async Task<GListTestsDataResponse> ListTestsData(GListTestsDataRequest gRequest, ServerCallContext context)
        {
            ListTestsDataRequest request = gRequest;

            IQueryable<TestCase> cases = Db.Cases
                .AsNoTracking()
                .OrderByDescending(c => c.CreationDate);
            if (request.TestIdsFilter.Length > 0)
            {
                var ids = request.TestIdsFilter.ToArray();
                IQueryable<TestCase> original = null;
                foreach (var filter in request.TestIdsFilter)
                {
                    var additional = cases.Where(c => c.TestId.StartsWith(filter));
                    original = original == null 
                        ? additional 
                        : original.Concat(additional);
                }
                cases = original;
            }
            var totalCount = await cases.CountAsync();
            if (request.Range != null)
            {
                var count = ((int)request.Range.To - (int)request.Range.From)
                    .NegativeToZero();
                cases = cases.Take(count);
            }
            
            var result = await cases.ToArrayAsync();

            return new ListTestsDataResponse(result, totalCount, Protobuf.StatusCode.Ok);
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
                await Db.Cases.AddAsync(new TestCase()
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
