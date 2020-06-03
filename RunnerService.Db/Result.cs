using RunnerService.API;
using RunnerService.API.Models;
using System.ComponentModel.DataAnnotations;
using Utilities.Types;

namespace RunnerService.Db
{
    public class Result
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Include(EntityGroups.ALL, EntityGroups.RESULTS)]
        public RunResultBase ResultBase { get; set; }
    }
}
