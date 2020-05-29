using PresentationService.API;
using System.Threading.Tasks;

namespace PresentationService
{
    public interface IMainHub
    {
        Task TestAdded(TestAddedWebMessage message);
        Task TestRecorded(TestRecordedWebMessage message);
        Task TestCompleted(TestCompletedWebMessage message);
        Task TestBegun(TestBegunWebMessage message);
        Task TestDeleted(TestDeletedWebMessage message);
        Task TestAddProgressChanged(TestAddProgressChangedWebMessage message);
    }
}
