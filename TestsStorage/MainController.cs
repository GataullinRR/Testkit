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
using Shared;
using MessageHub;
using Shared.Types;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;

namespace TestsStorageService
{
    [ApiController, Microsoft.AspNetCore.Mvc.Route("api/v1")]
    public class MainController : ControllerBase
    {
        [Inject] public IMessageProducer MessageProducer { get; set; }
        [Inject] public TestsContext Db { get; set; }
        
        public MainController(IDependencyResolver di)
        {
            di.ResolveProperties(this);
        }

        [Microsoft.AspNetCore.Mvc.Route("list"), HttpPost]
        public async Task<ListTestsDataResponse> ListTestsData(ListTestsDataRequest request)
        {
            IQueryable<TestCase> cases = Db.Cases.AsNoTracking()
                .IncludeGroup(EntityGroups.ALL, Db)
                .Where(c => request.ReturnNotSaved
                    ? c.State == TestCaseState.RecordedButNotSaved
                    : c.State == TestCaseState.Saved)
                .OrderByDescending(c => c.CreationDate);
            if (request.IsByIds)
            {
                cases = cases.Where(c => request.TestIds.Contains(c.TestId));
            }
            else if (request.IsByAuthorName)
            {
                cases = cases.Where(c => c.AuthorName == request.AuthorName);
            }
            else if (request.IsByNameFilters)
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
            else if (request.IsByParameters)
            {
                foreach (var fp in request.TestParameters)
                {
                    if (fp.Value != null)
                    {
                        cases = cases.Where(c => c.Data.KeyParameters.Any(p => p.Key == fp.Key && p.Value == fp.Value));
                    }
                }
            }
            else
            {
                throw new NotSupportedException();
            }

            var totalCount = await cases.CountAsync();
            if (request.Range != null)
            {
                var count = (request.Range.To - request.Range.From).NegativeToZero();
                cases = cases.Take(count);
            }
            
            var result = await cases.ToArrayAsync();

            return new ListTestsDataResponse(result, totalCount);
        }

        [Microsoft.AspNetCore.Mvc.Route("delete"), HttpPost]
        public async Task<DeleteTestResponse> DeleteTest(DeleteTestRequest request)
        {
            TestCase[] casesToDelete = null;
            var cases = Db.Cases.IncludeGroup(EntityGroups.ALL, Db);
            if (request.IsById)
            {
                var caseToDelete = await cases.FirstOrDefaultAsync(c => c.TestId == request.TestId);
                casesToDelete = caseToDelete == null
                    ? new TestCase[0]
                    : new TestCase[] { caseToDelete };
            }
            else
            {
                casesToDelete = await cases.Where(c => c.TestName != null && c.TestName.StartsWith(request.TestNameFilter)).ToArrayAsync();
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

            return new DeleteTestResponse();
        }

        [Microsoft.AspNetCore.Mvc.Route("save"), HttpPost]
        public async Task<IActionResult> SaveTest(SaveTestRequest request)
        {
            var test = await Db.Cases.FirstOrDefaultAsync(c => c.TestId == request.TestId);
            if (test?.State == TestCaseState.RecordedButNotSaved)
            {
                test.AuthorName = request.AuthorName;
                test.TestDescription = request.Description;
                test.TestName = request.Name;
                test.State = TestCaseState.Saved;
                await Db.SaveChangesAsync();

                MessageProducer.FireTestAdded(new TestAddedMessage(test.TestId, test.TestName, test.AuthorName));

                return Ok(new SaveTestResponse());
            }
            else
            {
                return NotFound();
            }
        }
    }
}
