using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utilities;
using Utilities.Extensions;

namespace Utilities.Types
{
    public class Timeouter
    {
        readonly Stopwatch _stopwatch = Stopwatch.StartNew();
        readonly int _timeout;
        readonly bool _reachable;

        public bool IsReached => _reachable && _stopwatch.Elapsed.TotalMilliseconds > _timeout;
        public int Left => _reachable 
            ? (_stopwatch.Elapsed.TotalMilliseconds - _timeout).ToInt32() 
            : 0;

        /// <summary>
        /// Throws if timeout has occurred.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public IDisposable PauseHolder
        {
            get
            {
                ThrowIfTimeout();
                _stopwatch.Stop();
                return new DisposingAction(() => _stopwatch.Start());
            }
        }

        /// <summary>
        /// Throws if timeout has occurred.
        /// </summary>
        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        public IDisposable RestartMode
        {
            get
            {
                ThrowIfTimeout();
                return new DisposingAction(() =>
                {
                    ThrowIfTimeout();
                    Restart();
                });
            }
        }

        public Timeouter(int timeout)
        {
            _timeout = timeout;
            _reachable = timeout != -1 && timeout != int.MaxValue;
        }

        /// <summary>
        /// Сбрасывает секундомер при условии, что таймаут еще не достигнут
        /// </summary>
        public void Reset()
        {
            if (!IsReached)
            {
                _stopwatch.Restart();
            }
        }

        /// <summary>
        /// Сбрасывает <see cref="IsReached"/> и начинает отсчет сначала
        /// </summary>
        public void Restart()
        {
            _stopwatch.Restart();
        }

        /// <summary>
        /// </summary>
        /// <exception cref="TimeoutException"></exception>
        public void ThrowIfTimeout()
        {
            if (IsReached)
            {
                throw new TimeoutException();
            }
        }

        /// <summary>
        /// </summary>
        /// <exception cref="TimeoutException"></exception>
        public void ThrowIfTimeout<T>(Func<T> exceptionFactory) 
            where T : TimeoutException
        {
            if (IsReached)
            {
                throw exceptionFactory();
            }
        }
    }
}
