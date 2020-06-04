using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace UserService.API
{
    public class GetUserInfoRequest
    {
        public string? Token { get; }
        public string? UserName { get; }

        public GetUserInfoRequest(string? token) : this(token, null)
        {

        }
        [JsonConstructor]
        public GetUserInfoRequest(string? token, string? userName)
        {
            Token = token;
            UserName = userName;
        }
    }
}
