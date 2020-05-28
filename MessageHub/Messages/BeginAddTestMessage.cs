using System;
using System.Collections.Generic;

namespace MessageHub
{
    public class BeginAddTestMessage
    {
        public string UserName { get; }
        public Dictionary<string, string> Parameters { get; }

        public BeginAddTestMessage(string userName, Dictionary<string, string> parameters)
        {
            UserName = userName ?? throw new ArgumentNullException(nameof(userName));
            Parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
        }
    }
}
