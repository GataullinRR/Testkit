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

            IQueryable<TestCase> cases = Db.Cases.AsNoTracking()
                .Where(c => request.ReturnNotSaved
                    ? c.State == TestCaseState.RecordedButNotSaved
                    : c.State == TestCaseState.Saved)
                .OrderByDescending(c => c.CreationDate);
            if (request.IsById)
            {
                cases = cases.Where(c => request.TestIds.Contains(c.TestId));
            }
            else
            {
                if (request.TestNameFilters.Length > 0)
                {
                    IQueryable<TestCase> original = null;
                    foreach (var filter in request.TestNameFilters)
                    {
                        var additional = cases.Where(c => c.TestName.StartsWith(filter));
                        original = original == null
                            ? additional
                            : original.Concat(additional);
                    }
                    cases = original;
                }
            }

            var totalCount = await cases.CountAsync();
            if (request.Range != null)
            {
                var count = (request.Range.To - request.Range.From).NegativeToZero();
                cases = cases.Take(count);
            }
            
            var result = await cases.ToArrayAsync();

            return new ListTestsDataResponse(result, totalCount, Protobuf.StatusCode.Ok);
        }

        public override async Task<GDeleteTestResponse> DeleteTest(GDeleteTestRequest gRequest, ServerCallContext context)
        {
            DeleteTestRequest request = gRequest;

            TestCase[] casesToDelete = null;
            if (request.IsById)
            {
                var caseToDelete = await Db.Cases.FirstOrDefaultAsync(c => c.TestId == request.TestId);
                casesToDelete = caseToDelete == null
                    ? new TestCase[0]
                    : new TestCase[] { caseToDelete };
            }
            else
            {
                casesToDelete = await Db.Cases.Where(c => c.TestName.StartsWith(request.TestNameFilter)).ToArrayAsync();
            }

            if (casesToDelete.Length > 0)
            {
                foreach (var caseToDelete in casesToDelete)
                {
                    Db.Cases.Remove(caseToDelete);
                    await Db.SaveChangesAsync();

                    MessageProducer.FireTestDeleted(new TestDeletedMessage(caseToDelete.TestId, caseToDelete.TestName));
                }
            }

            return new DeleteTestResponse(Protobuf.StatusCode.Ok);
        }

        public override async Task<GSaveTestResponse> SaveTest(GSaveTestRequest request, ServerCallContext context)
        {
            var test = await Db.Cases.FirstOrDefaultAsync(c => c.TestId == request.TestId);
            if (test.State == TestCaseState.RecordedButNotSaved)
            {
                test.DisplayName = request.DisplayName;
                test.TestName = request.TestName;
                test.State = TestCaseState.Saved;

                MessageProducer.FireTestAdded(new TestAddedMessage(test.TestId, test.TestName, test.AuthorName));

                await Db.SaveChangesAsync();
            };

            return new GSaveTestResponse()
            {
                Status = new GResponseStatus()
            };
        }
    }
}
