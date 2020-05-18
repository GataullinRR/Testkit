using System;
using System.Threading.Tasks;

namespace Runner
{
    public interface IUserChangeNotifier
    {
        event Func<Task> ProfileChangedAsync;
        event Func<Task> AuthStateChangedAsync;

        Task FireProfileChangedAsync();
        Task FireAuthStateChangedAsync();
    }
}
