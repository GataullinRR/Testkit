using Newtonsoft.Json;
using System;
using System.ComponentModel.DataAnnotations;
using Utilities.Types;

namespace UserService.API
{
    public class SignInRequest
    {
        [Required]
        public string UserName { get; }

        [Required, DataType(DataType.Password)]
        public string Password { get; }

        [JsonConstructor]
        public SignInRequest(string userName, string password)
        {
            UserName = userName ?? throw new ArgumentNullException(nameof(userName));
            Password = password ?? throw new ArgumentNullException(nameof(password));
        }
    }
}
