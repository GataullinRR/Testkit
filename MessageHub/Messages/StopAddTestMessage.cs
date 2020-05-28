using System;

namespace MessageHub
{
    public class StopAddTestMessage
    {
        public string UserName { get; }

        public StopAddTestMessage(string userName)
        {
            UserName = userName ?? throw new ArgumentNullException(nameof(userName));
        }
    }
}
