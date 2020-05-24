using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;
using Utilities.Types;

namespace UserService.API
{
    public class SignUpRequest
    {
        public static implicit operator global::UserService.API.GSignUpRequest(SignUpRequest request)
        {
            var gRequest = new global::UserService.API.GSignUpRequest()
            {
                UserName = request.UserName,
                Password = request.Password,
                EMail = request.Email,
                Phone = request.Phone,
            };

            return gRequest;
        }
        public static implicit operator SignUpRequest(global::UserService.API.GSignUpRequest request)
        {
            return new SignUpRequest(request.UserName, request.Password, request.Phone, request.EMail);
        }

        [Required]
        public string UserName { get; }

        [Required]
        public string Password { get; }

        [Required]
        public string Phone { get; }

        [Required]
        public string Email { get; }

        public SignUpRequest(string userName, string password, string phone, string email)
        {
            UserName = userName ?? throw new ArgumentNullException(nameof(userName));
            Password = password ?? throw new ArgumentNullException(nameof(password));
            Phone = phone ?? throw new ArgumentNullException(nameof(phone));
            Email = email ?? throw new ArgumentNullException(nameof(email));
        }
    }
}
