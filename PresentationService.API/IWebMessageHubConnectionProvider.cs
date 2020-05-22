using Microsoft.AspNetCore.SignalR.Client;
using System.Threading.Tasks;

namespace PresentationService.API
{
    public interface IWebMessageHubConnectionProvider
    {
        HubConnection Connection { get; }

        Task BindUserAsync(string token);
    }
}
