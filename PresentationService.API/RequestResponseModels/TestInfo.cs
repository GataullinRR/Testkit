using UserService.API;
using RunnerService.API;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestsStorageService.API;
using TestsStorageService.Db;
using RunnerService.API.Models;

namespace DDD
{
    public class TestInfo
    {
        public int TestId { get; set; }
        public string TestName { get; set; }
        public TestCaseState CreationState { get; set; }
        public GetUserInfoResponse Author { get; set; }
        public RunPlanBase RunPlan { get; set; }
        public StateBase State { get; set; }
        public RunResultBase? LastResult { get; set; }
        public TestCaseInfo Target { get; set; }
    }
}
