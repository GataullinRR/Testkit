using System.Threading.Tasks;

namespace Runner
{
    public interface IAppInitializationAwaiter
    {
        Task AwaitInitializedAsync();
    }
}
