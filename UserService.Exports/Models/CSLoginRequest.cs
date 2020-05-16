using System.ComponentModel.DataAnnotations;

namespace UserService.API
{
    public class CSLoginRequest
    {
        [Required]
        public virtual string UserName { get; set; }

        [Required, DataType(DataType.Password)]
        public virtual string Password { get; set; }
    }
}
