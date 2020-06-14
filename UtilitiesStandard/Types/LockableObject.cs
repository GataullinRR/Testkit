using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace Utilities.Types
{
    public class LockableObject<T>
    {
        public static implicit operator LockableObject<T>(T @object)
        {
            return new LockableObject<T>(@object);
        }

        public SemaphoreSlim Locker { get; } = new SemaphoreSlim(1);
        public T Object { get; }

        public LockableObject(T @object)
        {
            Object = @object;
        }
    }
}
