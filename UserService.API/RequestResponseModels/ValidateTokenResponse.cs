using System;
using System.Collections.Generic;
using System.Text;

namespace UserService.API
{
    public class ValidateTokenResponse
    {
        public bool IsValid { get; }

        public ValidateTokenResponse(bool isValid)
        {
            IsValid = isValid;
        }
    }
}
