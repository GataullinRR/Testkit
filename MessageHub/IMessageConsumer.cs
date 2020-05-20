using System;
using System.Threading.Tasks;

namespace MessageHub
{
    public interface IMessageConsumer
    {
        event Func<TestRecordedMessage, Task> TestRecordedAsync;
        event Func<TestExecutedMessage, Task> TestExecutedAsync;
        event Func<TestAcquiredMessage, Task> TestAcquiredAsync;
    }
}
