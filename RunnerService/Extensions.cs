using Microsoft.Extensions.Logging;
using SharedT.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace RunnerService
{
    public static class Extensions
    {
        public static IQueryable<Db.TestRunInfo> Filter<T>(this IQueryable<Db.TestRunInfo> runs, ILogger<T> logger, params IFilterOrder[] filteringOrders)
        {
            foreach (var order in filteringOrders)
            {
                if (order is ByTestIdsFilter idsFilter)
                {
                    runs = runs.Where(r => idsFilter.TestIds.Contains(r.TestId));
                }
                if (order is ByTestNamesFilter namesFilter)
                {
                    foreach (var nameFilter in namesFilter.TestNameFilters)
                    {
                        runs = runs.Where(r => r.TestName == nameFilter || r.TestName.StartsWith(nameFilter + "."));
                    }
                }
                else
                {
                    logger.LogWarning("Unsupported filter order {@Order}", order);
                }
            }

            return runs;
        }
    }
}
