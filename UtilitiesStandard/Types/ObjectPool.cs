using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utilities.Extensions;

namespace Utilities.Types
{
    /// <summary>
    /// Thread safe
    /// </summary>
    public class ObjectPool<T>
    {
        class ObjectInfo
        {
            public T Object;
            public bool IsInUse;
        }

        readonly Func<Task<T>> _factory;
        readonly int _maxPoolSize;
        readonly List<ObjectInfo> _objects = new List<ObjectInfo>();
        readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1);

        public ObjectPool(Func<Task<T>> factory, int maxPoolSize = int.MaxValue)
        {
            _factory = factory;
            _maxPoolSize = maxPoolSize;
        }

        public async Task<T> AquireAsync(AsyncOperationInfo operationInfo)
        {
            ObjectInfo objectToken = null;
            do
            {
                operationInfo.CancellationToken.ThrowIfCancellationRequested();
                objectToken = await tryAquireObjectAsync();
            }
            while (objectToken == null);

            return objectToken.Object; /////////////////////////////////////

            async Task<ObjectInfo> tryAquireObjectAsync()
            {
                using (await _semaphore.AcquireAsync(operationInfo))
                {
                    await populateIfAppropriateAsync();
                    return tryAcquire();
                }

                /////////////////////////////////////

                async Task populateIfAppropriateAsync()
                {
                    var canInstantiateNew = _maxPoolSize > _objects.Count;
                    var haveSpareObject = _objects.Any(s => !s.IsInUse);
                    if (!haveSpareObject && canInstantiateNew)
                    {
                        _objects.Add(new ObjectInfo()
                        {
                            IsInUse = false,
                            Object = await _factory()
                        });
                    }
                }

                ObjectInfo tryAcquire()
                {
                    foreach (var objectInfo in _objects)
                    {
                        if (!objectInfo.IsInUse)
                        {
                            objectInfo.IsInUse = true;

                            return objectInfo;
                        }
                    }

                    return null;
                }
            }
        }

        public async Task ReleaseAsync(T stream, AsyncOperationInfo operationInfo)
        {
            using (await _semaphore.AcquireAsync(operationInfo))
            {
                _objects.First(si => object.ReferenceEquals(si.Object, stream)).IsInUse = false;
            }
        }

        public async Task<T> InstantiateWithoutRegistering(AsyncOperationInfo operationInfo)
        {
            using (await _semaphore.AcquireAsync(operationInfo))
            {
                return await _factory();
            }
        }
    }
}
