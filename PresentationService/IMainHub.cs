using PresentationService.API;
using System.Threading.Tasks;

namespace PresentationService
{
    public interface IMainHub
    {
        Task TestRecorded(TestRecordedWebMessage message);
        Task TestCompleted(TestCompletedWebMessage message);
    }
}
