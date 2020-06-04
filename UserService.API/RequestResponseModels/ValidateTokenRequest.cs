using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace UserService.API
{
    public class ValidateTokenRequest
    {
        [Required]
        public string Token { get; }

        public ValidateTokenRequest(string token)
        {
            Token = token ?? throw new ArgumentNullException(nameof(token));
        }
    }
}
