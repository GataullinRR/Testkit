using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utilities.Extensions;

namespace Utilities.Types
{
    public class TaskCancellationManager : INotifyPropertyChanged
    {
        CancellationTokenSource _cts = new CancellationTokenSource();
        List<Task> _tasks = new List<Task>();

        public event PropertyChangedEventHandler PropertyChanged;

        public bool IsCancellationRequested => _cts.IsCancellationRequested;
        public bool HasTasks => _tasks.Count > 0;
        public CancellationToken Token => _cts.Token;

        public async Task WaitForAllToCancelAsync()
        {
            if (_cts.IsCancellationRequested)
            {
                for (int i = 0; i < _tasks.Count; i++)
                {
                    var task = _tasks[i];
                    try
                    {
                        await task;
                    }
                    catch (OperationCanceledException)
                    {

                    }
                    finally
                    {
                        _tasks.Remove(task);
                        i--;
                        PropertyChanged?.Invoke(this, nameof(HasTasks));
                    }
                }
            }
        }

        public Task RegisterAndReturn(Task task)
        {
            if (_cts.IsCancellationRequested)
            {
                throw new OperationCanceledException();
            }
            else
            {
                _tasks.Add(task);
                PropertyChanged?.Invoke(this, nameof(HasTasks));

                return task;
            }
        }

        public void Cancel()
        {
            if (!_cts.IsCancellationRequested)
            {
                _cts.Cancel();
                PropertyChanged?.Invoke(this, nameof(IsCancellationRequested));
            }
        }

        public void Reset()
        {
            _cts = new CancellationTokenSource();
            _tasks.Clear();
            PropertyChanged?.Invoke(this, nameof(IsCancellationRequested));
            PropertyChanged?.Invoke(this, nameof(HasTasks));
        }
    }
}
