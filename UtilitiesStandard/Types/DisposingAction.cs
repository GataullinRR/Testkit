using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Utilities;
using Utilities.Extensions;
using Utilities.Types;

namespace Utilities.Types
{
    public class DisposingAction : IDisposable
    {
        bool _isDisposed = false;
        Action _disposed;

        public DisposingAction(Action disposed)
        {
            _disposed = disposed;
        }

        void IDisposable.Dispose()
        {
            if (!_isDisposed)
            {
                _disposed();

                _isDisposed = true;
                _disposed = null;
            }
        }
    }
}
