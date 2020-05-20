using UserService.API;

namespace Runner
{
    public class Identity
    {
        public bool IsAuthentificated { get; }
        public CSUserInfo? User { get; }

        public Identity() : this(false, null)
        {
        
        }

        public Identity(bool isAuthentificated, CSUserInfo? user)
        {
            IsAuthentificated = isAuthentificated;
            User = user;
        }
    }
}
