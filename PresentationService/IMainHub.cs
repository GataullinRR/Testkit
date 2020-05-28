using PresentationService.API;
using System.Threading.Tasks;

namespace PresentationService
{
    public interface IMainHub
    {
        Task TestRecorded(TestAddedWebMessage message);
        Task TestCompleted(TestCompletedWebMessage message);
    }
}
