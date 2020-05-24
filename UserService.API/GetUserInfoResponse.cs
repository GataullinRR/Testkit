using Shared.Types;
using System;
using System.ComponentModel.DataAnnotations;
using UserService.API;
using Utilities.Types;

namespace UserService.API
{
    public class GetUserInfoResponse : ResponseBase
    {
        public static implicit operator global::UserService.API.GGetUserInfoResponse(GetUserInfoResponse response)
        {
            var gRequest = new global::UserService.API.GGetUserInfoResponse()
            {
                UserName = response.UserName,
                EMail = response.EMail,
                Phone = response.Phone,
                Status = response.Status,
            };

            return gRequest;
        }
        public static implicit operator GetUserInfoResponse(global::UserService.API.GGetUserInfoResponse response)
        {
            return new GetUserInfoResponse(response.UserName, response.EMail, response.Phone, response.Status);
        }

        [Required]
        public string UserName { get; set; }

        public string? EMail { get; set; }

        public string? Phone { get; set; }

        public GetUserInfoResponse(string userName, string? eMail, string? phone, ResponseStatus status) : base(status)
        {
            UserName = userName ?? throw new ArgumentNullException(nameof(userName));
            EMail = eMail;
            Phone = phone;
        }
    }
}
