using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Utilities.Extensions
{
    public static class QueryEx
    {
        public static async Task<T> FirstAndEnsurePopulatedAsync<T>(this DbSet<T> set, Expression<Func<T, bool>> predicate, T defaultEntity)
            where T : class
        {
            var value = await set.FirstOrDefaultAsync(predicate);
            if (value == null)
            {
                value = defaultEntity;
                await set.AddAsync(value);
            }

            return value;
        }
    }
}
