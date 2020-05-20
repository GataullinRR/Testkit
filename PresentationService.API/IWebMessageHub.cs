using System;
using System.Threading.Tasks;

namespace PresentationService.API
{
    public interface IWebMessageHub
    {
        event Func<TestRecordedWebMessage, Task> TestRecordedAsync;
    }
}
