using Microsoft.Extensions.DependencyInjection;
using Utilities.Types;
using System.Collections.ObjectModel;

namespace Runner
{
    [Service(ServiceLifetime.Scoped)]
    class MessageService : IMessageService
    {
        public ObservableCollection<Message> Messages { get; } = new ObservableCollection<Message>();
    }
}
