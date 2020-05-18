using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Runner
{
    public class TestInfo
    {
        public int Id { get; set; }
        public string Description { get; set; }

        public Author Author { get; set; }
        public RunPlanBase RunPlan { get; set; }
        public StateBase State { get; set; }
        public RunResultBase? LastResult { get; set; }
        public TargetBase Target { get; set; }
    }
}
