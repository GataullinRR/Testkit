using UserService.API;
using Utilities.Types;

namespace UserService.API
{
    [AutoMapFrom(typeof(GetUserInfoResponse))]
    public class CSUserInfo
    {
        public string UserName { get; set; }
        public string? EMail { get; set; }
        public string? Phone { get; set; }
    }
}
