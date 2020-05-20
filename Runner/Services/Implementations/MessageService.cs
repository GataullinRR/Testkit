using Microsoft.Extensions.DependencyInjection;
using Utilities.Types;
using System.Collections.ObjectModel;

namespace Runner
{
    [Service(ServiceLifetime.Singleton)]
    class MessageService : IMessageService
    {
        public ObservableCollection<Message> Messages { get; } = new ObservableCollection<Message>();
    }
}
