using System;
using System.Threading.Tasks;

namespace PresentationService.API
{
    public interface IWebMessageHub
    {
        event Func<TestAddedWebMessage, Task> TestAddedAsync;
        event Func<TestCompletedWebMessage, Task> TestCompletedAsync;
    }
}
