using SharedT.Types;
using System;
using System.ComponentModel.DataAnnotations;
using UserService.API;
using Utilities.Types;

namespace UserService.API
{
    public class GetUserInfoResponse
    {
        [Required]
        public string UserName { get; }

        public string? EMail { get; }

        public string? Phone { get; }

        public GetUserInfoResponse(string userName, string? eMail, string? phone)
        {
            UserName = userName ?? throw new ArgumentNullException(nameof(userName));
            EMail = eMail;
            Phone = phone;
        }
    }
}
