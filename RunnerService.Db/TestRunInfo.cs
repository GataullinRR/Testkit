using RunnerService.API;
using RunnerService.API.Models;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Utilities.Types;

namespace RunnerService.Db
{
    public class TestRunInfo
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.None)]
        public int TestId { get; set; }

        [Required]
        public string TestName { get; set; }

        [Required]
        [Include(EntityGroups.ALL, EntityGroups.RESULTS)]
        public List<Result> Results { get; set; } = new List<Result>();
        
        [Required]
        [Include(EntityGroups.ALL)]
        public RunPlanBase RunPlan { get; set; }

        public TestRunInfo() { } // For EF
    }
}
