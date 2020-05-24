using System;
using System.ComponentModel.DataAnnotations;
using Utilities.Types;

namespace UserService.API
{
    public class SignInRequest
    {
        public static implicit operator global::UserService.API.GSignInRequest(SignInRequest request)
        {
            var gRequest = new global::UserService.API.GSignInRequest()
            {
                UserName = request.UserName,
                Password = request.Password,
            };

            return gRequest;
        }
        public static implicit operator SignInRequest(global::UserService.API.GSignInRequest request)
        {
            return new SignInRequest(request.UserName, request.Password);
        }

        [Required]
        public string UserName { get; }

        [Required, DataType(DataType.Password)]
        public string Password { get; }

        public SignInRequest(string userName, string password)
        {
            UserName = userName ?? throw new ArgumentNullException(nameof(userName));
            Password = password ?? throw new ArgumentNullException(nameof(password));
        }
    }
}
