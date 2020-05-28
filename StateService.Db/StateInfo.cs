using System.ComponentModel.DataAnnotations;

namespace StateService.Db
{
    public class StateInfo
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string UserName { get; set; }
        
        [Required]
        public UserState State { get; set; }
    }
}
