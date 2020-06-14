using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Utilities;

namespace Utilities.Types
{
    public class AsyncOperationInfo
    {
        public static implicit operator AsyncOperationInfo(CancellationToken cancellation)
        {
            return new AsyncOperationInfo(new RichProgress(), cancellation);
        }
        public static implicit operator CancellationToken(AsyncOperationInfo operationInfo)
        {
            return operationInfo.CancellationToken;
        }

        public RichProgress Progress { get; }
        public CancellationToken CancellationToken => _userToken ?? _cts?.Token ?? CancellationToken.None;

        CancellationToken? _userToken;
        CancellationTokenSource _cts;

        public AsyncOperationInfo()
            : this(new RichProgress(), null)
        {

        }
        public AsyncOperationInfo(RichProgress progress)
            : this(progress, null)
        {

        }
        public AsyncOperationInfo(RichProgress progress, int timeout)
            : this(progress, ThreadingUtils.CreateCToken(timeout))
        {

        }
        public AsyncOperationInfo(RichProgress progress, CancellationToken? cancellationToken)
        {
            Progress = progress ?? throw new ArgumentNullException(nameof(progress));
            _userToken = cancellationToken;
        }

        public AsyncOperationInfo UseInternalCancellationSource()
        {
            if (_userToken != null)
            {
                throw new InvalidOperationException();
            }

            _cts = new CancellationTokenSource();

            return this;
        }

        public void Cancel()
        {
            _cts.Cancel();
        }
    }
}
