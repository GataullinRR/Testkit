namespace Runner
{
    public class Identity
    {
        public bool IsAuthentificated { get; }
        public UserInfo? User { get; }

        public Identity() : this(false, null)
        {
        
        }

        public Identity(bool isAuthentificated, UserInfo? user)
        {
            IsAuthentificated = isAuthentificated;
            User = user;
        }
    }
}
