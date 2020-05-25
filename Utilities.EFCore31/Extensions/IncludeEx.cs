using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Utilities.Types;

namespace Utilities.Extensions
{
    public static class IncludeEx
    {
        public static IQueryable<T> IncludeGroup<T>(this IQueryable<T> set, string groupName, DbContext context) where T : class
        {
            include(typeof(T), null);

            return set;

            void include(Type t, string perfix)
            {
                var target = typeof(IEnumerable).IsAssignableFrom(t)
                    ? t.GenericTypeArguments.Single()
                    : t;
                var propertiesToInclude = target
                    .GetProperties()
                    .Where(p => p.GetCustomAttribute<IncludeAttribute>()?.Groups?.Contains(groupName) ?? false);
                foreach (var p in propertiesToInclude)
                {
                    var pLocator = perfix == null
                        ? p.Name
                        : $"{perfix}.{p.Name}";
                    set = set.Include(pLocator);

                    include(p.PropertyType, pLocator);
                }
            }
        }
    }
}
