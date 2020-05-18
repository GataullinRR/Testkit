using System.Collections.ObjectModel;

namespace Runner
{
    public interface IMessageService
    {
        ObservableCollection<Message> Messages { get; }
    }
}
