using System;
using System.Threading.Tasks;

namespace Runner
{
    public interface IUserChangeNotifier
    {
        event Func<Task> ProfileChangedAsync;
        event Func<Task> AuthStateChangedAsync;
        event Func<Task> IdentityChangedAsync;

        Task FireProfileChangedAsync();
        Task FireAuthStateChangedAsync();
        Task FireIdentityChangedAsync();
    }
}
