using System;
using System.Threading.Tasks;

namespace MessageHub
{
    public interface IMessageConsumer
    {
        event Func<TestAddedMessage, Task> TestAddedAsync;
        event Func<TestExecutedMessage, Task> TestExecutedAsync;
        event Func<TestAcquiringResultMessage, Task> TestAcquiredAsync;
        event Func<TestCompletedMessage, Task> TestCompletedAsync;
        event Func<TestCompletedOnSourceMessage, Task> TestCompletedOnSourceAsync;
        event Func<TestDeletedMessage, Task> TestDeletedAsync;
        event Func<TestRecordedMessage, Task> TestRecordedAsync;

        event Func<BeginTestMessage, Task> BeginTestAsync;
        event Func<CancelTestMessage, Task> CancelTestAsync;
        event Func<TestCancelledMessage, Task> TestCancelledAsync;

        event Func<UpdateTestResultStateMessage, Task> UpdateTestResultAsync;
        event Func<TestResultStateAcquiredMessage, Task> TestResultStateAcquiredAsync;
        event Func<TestResultStateUpdatedMessage, Task> TestResultStateUpdatedAsync;
    }
}
