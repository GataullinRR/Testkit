using RunnerService.APIModels;
using System.ComponentModel.DataAnnotations;

namespace RunnerService.Db
{
    public class TestRunInfo
    {
        [Key]
        public string TestId { get; set; }

        public RunResultBase LastRun { get; set; }
        
        [Required]
        public StateBase State { get; set; }

        [Required]
        public RunPlanBase RunPlan { get; set; }
    }
}
