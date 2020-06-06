using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;

namespace WebNotificationService.API
{
    public interface IWebMessageHubConnectionProvider
    {
        HubConnection Connection { get; }

        Task BindUserAsync(string token);
    }
}
