using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.Extensions;

namespace Utilities.Types
{
    public class EnhancedEnumerator<T> : IEnumerator<T>
    {
        readonly IEnumerator<T> _enumerator;
        T _next;
        bool _hasNext;
        bool _firstMove = true;
        bool _disposed;
        bool _enumeratingCompleted;

        T _Current;
        bool _IsLastElement;
        int _Index = -1;

        public T Current
        {
            get
            {
                throwIfDisposedOrFinishedOrNotStarted();
                return _Current;
            }
            private set => _Current = value;
        }
        object IEnumerator.Current => Current;

        public bool IsLastElement
        {
            get
            {
                throwIfDisposedOrFinishedOrNotStarted();
                return _IsLastElement;
            }
            private set => _IsLastElement = value;
        }
        public int Index
        {
            get
            {
                throwIfDisposedOrFinishedOrNotStarted();
                return _Index;
            }
            private set => _Index = value;
        }

        void throwIfDisposedOrFinishedOrNotStarted()
        {
            if (_enumeratingCompleted || _firstMove)
            {
                throw new InvalidOperationException();
            }

            throwIfDisposed();
        }
        void throwIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(EnhancedEnumerator<T>));
            }
        }

        public EnhancedEnumerator(IEnumerable<T> sequence)
            : this(sequence.GetEnumerator())
        {

        }
        EnhancedEnumerator(IEnumerator<T> enumerator)
        {
            _enumerator = enumerator;
        }

        public void Break()
        {
            Dispose();
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _enumerator.Dispose();
                _disposed = true;
            }
        }

        public bool MoveNext()
        {
            throwIfDisposed();

            if (_firstMove)
            {
                _hasNext = _enumerator.MoveNext();
                if (_hasNext)
                {
                    _next = _enumerator.Current;
                }

                _firstMove = false;
            }

            if (_hasNext)
            {
                Current = _next;
            }
            else
            {
                _enumeratingCompleted = true;
                return false;
            }

            _hasNext = _enumerator.MoveNext();
            if (_hasNext)
            {
                _next = _enumerator.Current;
            }
            else
            {
                IsLastElement = true;
            }

            Index++;
            return true;
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }
    }
}
