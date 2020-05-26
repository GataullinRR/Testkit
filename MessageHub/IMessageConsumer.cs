using System;
using System.Threading.Tasks;

namespace MessageHub
{
    public interface IMessageConsumer
    {
        event Func<TestRecordedMessage, Task> TestRecordedAsync;
        event Func<TestExecutedMessage, Task> TestExecutedAsync;
        event Func<TestAcquiringResultMessage, Task> TestAcquiredAsync;
        event Func<TestCompletedMessage, Task> TestCompletedAsync;
        event Func<TestCompletedOnSourceMessage, Task> TestCompletedOnSourceAsync;
        event Func<TestDeletedMessage, Task> TestDeletedAsync;

        event Func<BeginTestMessage, Task> BeginTestAsync;
    }
}
