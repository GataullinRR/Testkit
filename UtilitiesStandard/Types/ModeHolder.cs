using System;
using System.Collections.Generic;

namespace Utilities.Types
{
    public interface IModeHolder<out TOwner> : IDisposable
    {
        TOwner Owner { get; }
        event Action<TOwner> Disposed;
    }

    public sealed class ModeHolder<TOwner> : Disposable, IModeHolder<TOwner>
    {
        readonly List<IDisposable> _disposables = new List<IDisposable>();

        public event Action<TOwner> Disposed;

        public TOwner Owner { get; }

        public ModeHolder(IDisposable modeRevertor, TOwner owner)
        {
            _disposables.Add(modeRevertor ?? throw new ArgumentNullException(nameof(modeRevertor)));

            Owner = owner;
        }

        public ModeHolder<TOwner> ThenDispose(IDisposable disposable)
        {
            _disposables.Add(disposable);

            return this;
        }

        protected override void DisposeManagedState()
        {
            foreach (var disposable in _disposables)
            {
                disposable.Dispose();
            }

            Disposed?.Invoke(Owner);
        }
    }
}
