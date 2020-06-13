using System;
using System.Threading.Tasks;

namespace WebNotificationService.API
{
    public interface IWebMessageHub
    {
        event Func<TestAddedWebMessage, Task> TestAddedAsync;
        event Func<TestDeletedWebMessage, Task> TestDeletedAsync;
        event Func<TestBegunWebMessage, Task> TestBegunAsync;
        event Func<TestCancelledWebMessage, Task> TestCancelledAsync;
        event Func<TestCompletedWebMessage, Task> TestCompletedAsync;
        event Func<TestRecordedWebMessage, Task> TestRecordedAsync;
        event Func<EntryChangedWebMessageBase, Task> EntryChangedAsync;
    }
}
