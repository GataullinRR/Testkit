using System;
using System.Threading.Tasks;

namespace PresentationService.API
{
    public interface IWebMessageHub
    {
        event Func<TestAddedWebMessage, Task> TestAddedAsync;
        event Func<TestCompletedWebMessage, Task> TestCompletedAsync;
        event Func<TestDeletedWebMessage, Task> TestDeletedAsync;
        event Func<TestBegunWebMessage, Task> TestBegunAsync;
        event Func<TestRecordedWebMessage, Task> TestRecordedAsync;
        event Func<TestAddProgressChangedWebMessage, Task> TestAddProgressChangedAsync;
    }
}
