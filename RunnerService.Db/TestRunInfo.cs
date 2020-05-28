using RunnerService.APIModels;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Utilities.Types;

namespace RunnerService.Db
{
    public class TestRunInfo
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public int TestId { get; set; }

        [Required]
        public string TestName { get; set; }

        [Required]
        [Include(EntityGroups.ALL, EntityGroups.RESULTS)]
        public List<Result> Results { get; set; } = new List<Result>();

        [Required]
        [Include(EntityGroups.ALL)]
        public StateBase State { get; set; }
        
        [Required]
        [Include(EntityGroups.ALL)]
        public RunPlanBase RunPlan { get; set; }

        public TestRunInfo() { } // For EF
    }
}
