using System.Threading.Tasks;
using WebNotificationService.API;

namespace WebNotificationService
{
    public interface IMainHub
    {
        Task TestAdded(TestAddedWebMessage message);
        Task TestCompleted(TestCompletedWebMessage message);
        Task TestBegun(TestBegunWebMessage message);
        Task TestDeleted(TestDeletedWebMessage message);
        Task TestRecorded(TestRecordedWebMessage message);
        Task TestCancelled(TestCancelledWebMessage message);
    }
}
