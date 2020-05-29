namespace MessageHub
{
    public class TestAddProgressChangedMessage
    {
        public string UserName { get; }
        public string Log { get; }

        public TestAddProgressChangedMessage(string userName, string log)
        {
            UserName = userName;
            Log = log;
        }
    }
}
