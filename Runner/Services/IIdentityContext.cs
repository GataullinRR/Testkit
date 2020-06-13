using System.Threading.Tasks;

namespace Runner
{
    public interface IIdentityContext
    {
        Identity Identity { get; }

        Task LoadAsync();
    }
}
