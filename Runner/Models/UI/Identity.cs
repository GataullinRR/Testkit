using UserService.API;

namespace Runner
{
    public class Identity
    {
        public bool IsAuthentificated { get; }
        public GetUserInfoResponse? User { get; }

        public Identity() : this(false, null)
        {
        
        }

        public Identity(bool isAuthentificated, GetUserInfoResponse? user)
        {
            IsAuthentificated = isAuthentificated;
            User = user;
        }
    }
}
