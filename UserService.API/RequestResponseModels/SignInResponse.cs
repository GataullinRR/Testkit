using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace UserService.API
{
    public class SignInResponse
    {
        [Required]
        public string Token { get; }

        public SignInResponse(string token)
        {
            Token = token ?? throw new ArgumentNullException(nameof(token));
        }
    }
}
