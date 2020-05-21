using Microsoft.Extensions.DependencyInjection;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Utilities.Extensions;
using Utilities.Types;

namespace MessageHub
{
    [Service(ServiceLifetime.Transient)]
    class MessageHubOptions
    {
        public string ServerURI { get; set; } = "localhost:9092";
        public string TestRecordedTopic { get; set; } = "test-recorded";
        public string TestExecutedTopic { get; set; } = "test-executed";
        public string TestAcquiredTopic { get; set; } = "test-acquired";
        public string TestCompletedTopic { get; set; } = "test-completed";
        public string TestCompletedOnSourceTopic { get; set; } = "test-completed-on-source";
    }
}
