using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utilities.Extensions;

namespace Utilities.Types
{
    public class TaskHolder
    {
        readonly SemaphoreSlim _locker = new SemaphoreSlim(1);
        readonly CancellationTokenSource _cts = new CancellationTokenSource();
        readonly List<Task> _tasks = new List<Task>();

        public bool IsFinished { get; set; }

        public async Task RegisterAsync(Func<CancellationToken, Task> task)
        {
            using (await _locker.AcquireAsync())
            {
                throwIfFinished();

                _tasks.Add(task(_cts.Token));
            }
        }

        public void Cancel()
        {
            throwIfFinished();

            _cts.Cancel();
        }

        public async Task WaitAllAsync()
        {
            using (await _locker.AcquireAsync())
            {
                throwIfFinished();

                var exceptions = new List<Exception>();
                foreach (var t in _tasks)
                {
                    try
                    {
                        await t;
                    }
                    catch (Exception ex)
                    {
                        exceptions.Add(ex);
                    }
                }

                _tasks.Clear();
                if (exceptions.Count != 0)
                {
                    throw new AggregateException(exceptions);
                }

                IsFinished = true;
            }
        }

        void throwIfFinished()
        {
            if (IsFinished)
            {
                throw new InvalidOperationException();
            }
        }
    }
}
