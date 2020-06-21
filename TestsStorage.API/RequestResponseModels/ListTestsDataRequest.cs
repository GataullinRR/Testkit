using Newtonsoft.Json;
using SharedT.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Utilities.Extensions;
using Vectors;

namespace TestsStorageService.API
{
    public class ListTestsDataRequest
    {
        [Required]
        public IFilterOrder[] FilteringOrders { get; }

        [JsonConverter(typeof(IntIntervalSerializer))]
        public IntInterval Range { get; }
        public bool IncludeData { get; }
        public bool ReturnNotSaved { get; }

        public ListTestsDataRequest(IFilterOrder filteringOrder, IntInterval range, bool includeData, bool returnNotSaved) 
            : this(new [] { filteringOrder }, range, includeData, returnNotSaved)
        {

        }
        public ListTestsDataRequest(IFilterOrder[] filteringOrders, IntInterval range, bool includeData, bool returnNotSaved)
        {
            FilteringOrders = filteringOrders ?? throw new ArgumentNullException(nameof(filteringOrders));
            Range = range;
            IncludeData = includeData;
            ReturnNotSaved = returnNotSaved;
        }

        public static ListTestsDataRequest ByIds(int[] ids, IntInterval range, bool returnNotSaved, bool includeData)
        {
            return new ListTestsDataRequest(new ByTestIdsFilter(ids) , range, returnNotSaved, includeData);
        }
        public static ListTestsDataRequest ByAuthorName(string authorName, IntInterval range, bool returnNotSaved, bool includeData)
        {
            return new ListTestsDataRequest(new ByAuthorsFilter(new[] { authorName }), range, returnNotSaved, includeData);
        }
        public static ListTestsDataRequest ByNameFilter(string[] nameFilters, IntInterval range, bool returnNotSaved, bool includeData)
        {
            return new ListTestsDataRequest(new ByTestNamesFilter(nameFilters), range, returnNotSaved, includeData);
        }
        public static ListTestsDataRequest ByParameters(Dictionary<string, string> testParameters, IntInterval range, bool returnNotSaved, bool includeData)
        {
            return new ListTestsDataRequest(new ByKeyParametersFilter(testParameters), range, returnNotSaved, includeData);
        }
        public static ListTestsDataRequest ByQuery(string query, IntInterval range, bool returnNotSaved, bool includeData)
        {
            return new ListTestsDataRequest(new ByQueryFilter(query), range, returnNotSaved, includeData);
        }
    }
}
