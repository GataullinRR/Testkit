using Microsoft.AspNetCore.SignalR.Client;

namespace PresentationService.API
{
    public interface IWebMessageHubConnectionProvider
    {
        HubConnection Connection { get; }
    }
}
