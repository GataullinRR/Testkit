using UserService.API;
using RunnerService.API;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TestsStorageService.API;

namespace DDD
{
    public class TestInfo
    {
        public string TestId { get; set; }
        public CSUserInfo Author { get; set; }
        public RunPlanBase RunPlan { get; set; }
        public StateBase State { get; set; }
        public RunResultBase? LastResult { get; set; }
        public CSTestCaseInfo Target { get; set; }
    }
}
