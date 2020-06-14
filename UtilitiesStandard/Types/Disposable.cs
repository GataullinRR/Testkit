using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.Extensions;

namespace Utilities.Types
{
    public abstract class Disposable : IDisposable
    {
        bool _disposed;

        protected void throwIfDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        protected abstract void DisposeManagedState();

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                DisposeManagedState();
            }
        }
    }
}
