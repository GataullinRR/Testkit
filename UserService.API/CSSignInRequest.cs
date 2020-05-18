using System.ComponentModel.DataAnnotations;
using Utilities.Types;

namespace UserService.API
{
    [AutoMapFrom(typeof(SignInRequest))]
    [AutoMapTo(typeof(SignInRequest))]
    public class CSSignInRequest
    {
        [Required]
        public virtual string UserName { get; set; }

        [Required, DataType(DataType.Password)]
        public virtual string Password { get; set; }
    }
}
