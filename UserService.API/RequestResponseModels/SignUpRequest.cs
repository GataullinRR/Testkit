using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Utilities.Types;

namespace UserService.API
{
    public class SignUpRequest
    {
        [Required]
        public string UserName { get; }

        [Required]
        public string Password { get; }

        [Required]
        public string Phone { get; }

        [Required]
        public string EMail { get; }

        [JsonConstructor]
        public SignUpRequest(string userName, string password, string phone, string email)
        {
            UserName = userName ?? throw new ArgumentNullException(nameof(userName));
            Password = password ?? throw new ArgumentNullException(nameof(password));
            Phone = phone ?? throw new ArgumentNullException(nameof(phone));
            EMail = email ?? throw new ArgumentNullException(nameof(email));
        }
    }
}
