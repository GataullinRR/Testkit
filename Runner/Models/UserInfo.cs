using UserService.API;
using Utilities.Types;

namespace Runner
{
    [AutoMapFrom(typeof(GetUserInfoResponse))]
    public class UserInfo
    {
        public string UserName { get; set; }
        public string? EMail { get; set; }
        public string? Phone { get; set; }
    }
}
