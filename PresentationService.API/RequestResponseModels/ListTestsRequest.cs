using Newtonsoft.Json;
using SharedT.Types;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using TestsStorageService.API;
using Vectors;

namespace PresentationService.API
{
    public class ListTestsRequest
    {
        [Required]
        public IFilterOrder[] FilteringOrders { get; }

        [JsonConverter(typeof(IntIntervalSerializer))]
        public IntInterval Range { get; }
        public bool ReturnNotSaved { get; }

        public ListTestsRequest(IFilterOrder[] filteringOrders, IntInterval range, bool returnNotSaved)
        {
            FilteringOrders = filteringOrders ?? throw new ArgumentNullException(nameof(filteringOrders));
            Range = range;
            ReturnNotSaved = returnNotSaved;
        }

        public static ListTestsRequest ByIdsName(int[] ids, IntInterval range, bool returnNotSaved)
        {
            return new ListTestsRequest(new[] { new ByTestIdsFilter(ids) }, range, returnNotSaved);
        }
        public static ListTestsRequest ByAuthorName(string authorName, IntInterval range, bool returnNotSaved)
        {
            return new ListTestsRequest(new[] { new ByAuthorsFilter(new[] { authorName }) }, range, returnNotSaved);
        }
        public static ListTestsRequest ByNameFilter(string nameFilter, IntInterval range, bool returnNotSaved)
        {
            return new ListTestsRequest(new[] { new ByTestNamesFilter(new[] { nameFilter }) }, range, returnNotSaved);
        }
        public static ListTestsRequest ByParameters(Dictionary<string, string> testParameters, IntInterval range, bool returnNotSaved)
        {
            return new ListTestsRequest(new[] { new ByKeyParametersFilter(testParameters) }, range, returnNotSaved);
        }
        public static ListTestsRequest ByQuery(string query, IntInterval range, bool returnNotSaved)
        {
            return new ListTestsRequest(new[] { new ByQueryFilter(query) }, range, returnNotSaved);
        }
    }
}
