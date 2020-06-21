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
using SharedT;
using MessageHub;
using SharedT.Types;
using System.Linq.Expressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace TestsStorageService
{
    [ApiController, Microsoft.AspNetCore.Mvc.Route("api/v1")]
    public class MainController : ControllerBase
    {
        [Inject] public IMessageProducer MessageProducer { get; set; }
        [Inject] public TestsContext Db { get; set; }
        [Inject] public ILogger<MainController> Logger { get; set; }
        
        public MainController(IDependencyResolver di)
        {
            di.ResolveProperties(this);
        }

        [Microsoft.AspNetCore.Mvc.Route("list"), HttpPost]
        public async Task<ListTestsDataResponse> ListTestsData(ListTestsDataRequest request)
        {
            IQueryable<TestCase> cases = Db.Cases
                .AsNoTracking()
                .IncludeGroup(EntityGroups.ALL, Db)
                .Where(c => request.ReturnNotSaved
                    ? c.State == TestCaseState.RecordedButNotSaved
                    : c.State == TestCaseState.Saved)
                .OrderByDescending(c => c.CreationDate);
            cases = filterCases(cases, request.FilteringOrders);
            
            var totalCount = cases is Microsoft.EntityFrameworkCore.Query.Internal.IAsyncQueryProvider
                ? await cases.CountAsync()
                : cases.Count();
            if (request.Range != null)
            {
                var count = (request.Range.To - request.Range.From).NegativeToZero();
                cases = cases.Take(count);
            }

            var result = cases is Microsoft.EntityFrameworkCore.Query.Internal.IAsyncQueryProvider
                ? await cases.ToArrayAsync()
                : cases.ToArray();
            if (!request.IncludeData)
            {
                foreach (var r in result)
                {
                    r.Data.Data = null;
                }
            }

            return new ListTestsDataResponse(result, totalCount);
        }

        [Microsoft.AspNetCore.Mvc.Route("delete"), HttpPost]
        public async Task<DeleteTestResponse> DeleteTest(DeleteTestRequest request)
        {
            var cases = Db.Cases.IncludeGroup(EntityGroups.ALL, Db);
            var casesToDelete = await filterCases(cases, request.FilteringOrders)
                .ToArrayAsync();

            if (casesToDelete.Length > 0)
            {
                foreach (var caseToDelete in casesToDelete)
                {
                    Db.Cases.Remove(caseToDelete);
                    await Db.SaveChangesAsync(); // i know...

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

        IQueryable<TestCase> filterCases(IQueryable<TestCase> cases, IFilterOrder[] filteringorders)
        {
            foreach (var filterOrder in filteringorders)
            {
                if (filterOrder is ByTestIdsFilter ids)
                {
                    cases = cases.Where(c => ids.TestIds.Contains(c.TestId));
                }
                else if (filterOrder is ByTestNamesFilter namesFilter)
                {
                    foreach (var nameFilter in namesFilter.TestNameFilters)
                    {
                        cases = cases
                            .Where(r => r.TestName == nameFilter
                                || r.TestName.StartsWith(nameFilter + "."));
                    }
                }
                else if (filterOrder is ByKeyParametersFilter parameters)
                {
                    foreach (var parameter in parameters.TestParameters)
                    {
                        if (parameter.Value != null)
                        {
                            cases = cases
                                .Where(c => c.Data.KeyParameters
                                    .Any(p => p.Key == parameter.Key && p.Value == parameter.Value));
                        }
                    }
                }
                else if (filterOrder is ByQueryFilter query)
                {
                    if (query.Query.IsNotNullOrEmpty() && query.Query.IsNotNullOrWhiteSpace())
                    {
                        var queryKeywords = query.Query
                            .Split(' ', StringSplitOptions.RemoveEmptyEntries)
                            .Take(6)
                            .ToArray();
                        cases = cases
                            .AsEnumerable()
                            .Select(c => new
                            {
                                Case = c,
                                Keywords = c.Data.KeyParameters
                                    .Select(p => p.Value)
                                    .Concat(c.TestDescription.Split(' ', StringSplitOptions.RemoveEmptyEntries))
                                    .Concat(new[] { c.TestName })
                                    .ToArray(),
                            })
                            .Select(c => new
                            {
                                Case = c.Case,
                                Match = c.Keywords.Select(k => queryKeywords.Select(qk => k.FindAll(qk).Count() * qk.Length).Sum()).Sum() / (double)c.Keywords.Sum(k => k.Length)
                            })
                            .Where(c => c.Match != 0)
                            .OrderByDescending(c => c.Match)
                            .Select(c => c.Case)
                            .AsQueryable();
                    }
                }
                else if (filterOrder is ByAuthorsFilter authors)
                {
                    foreach (var author in authors.AuthorNames)
                    {
                        cases = cases.Where(c => c.AuthorName == author);
                    }
                }
                else
                {
                    Logger.LogWarning("Unsupported filter order {@FilterOrder}", filterOrder);
                }
            }

            return cases;
        }
    }
}
