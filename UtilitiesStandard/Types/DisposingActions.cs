using System;
using System.Collections;
using System.Collections.Generic;

namespace Utilities.Types
{
    public class DisposingActions : IEnumerable<Action>, IDisposable
    {
        bool _isDisposed = false;
        readonly List<Action> _disposed = new List<Action>();

        public DisposingActions Add(Action action)
        {
            _disposed.Add(action);
            return this;
        }
        public DisposingActions Add(IDisposable action)
        {
            _disposed.Add(action.Dispose);
            return this;
        }
        public DisposingActions Add(IEnumerable<Action> action)
        {
            _disposed.AddRange(action);
            return this;
        }

        void IDisposable.Dispose()
        {
            if (!_isDisposed)
            {
                var exceptions = new List<Exception>();
                foreach (var disposeAction in _disposed)
                {
                    try
                    {
                        disposeAction();
                    }
                    catch (Exception ex)
                    {
                        exceptions.Add(ex);
                    }
                }

                _isDisposed = true;

                if (exceptions.Count != 0)
                {
                    throw new AggregateException(exceptions);
                }
            }
        }

        public IEnumerator<Action> GetEnumerator()
        {
            return _disposed.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return _disposed.GetEnumerator();
        }
    }
}
