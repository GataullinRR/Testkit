using UserService.API;
using RunnerService.APIModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestsStorageService.API;
using TestsStorageService.Db;

namespace DDD
{
    public class TestInfo
    {
        public string TestId { get; set; }
        public TestCaseState CreationState { get; set; }
        public GetUserInfoResponse Author { get; set; }
        public RunPlanBase RunPlan { get; set; }
        public StateBase State { get; set; }
        public RunResultBase? LastResult { get; set; }
        public CSTestCaseInfo Target { get; set; }
    }
}
