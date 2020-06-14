using System;
using System.IO;

namespace Utilities.Types
{
    public class DisposalNotificationStreamProxy : StreamProxyBase
    {
        public event Action Disposing;
        public event Action Disposed;

        public DisposalNotificationStreamProxy(Stream baseStream) : base(baseStream)
        {

        }

        protected override void Dispose(bool disposing)
        {
            Disposing();
            base.Dispose(disposing);
            Disposed();
        }
    }
}
