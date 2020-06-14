using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Utilities;
using Utilities.Extensions;
using Utilities.Types;

namespace Utilities.Extensions
{
    public static class ThreadingEx
    {
        public static Task<T> AsCompletedTask<T>(this T value)
        {
            return Task.FromResult(value);
        }

        public static void Send(this SynchronizationContext synchronizationContext,  Action action)
        {
            synchronizationContext.Send(o => action(), null);
        }

        public static async Task<T[]> ToArrayAsync<T>(this IEnumerable<T> sequence)
        {
            if (sequence is T[] array)
            {
                return array;
            }
            else
            {
                return await Task.Run(() => sequence.ToArray());
            }   
        }

        //public static Task NoThrowWhenCanceled(this Task continuation)
        //{
        //    return continuation.ContinueWith(
        //        ant => { var ex = ant.Exception; }, TaskContinuationOptions.OnlyOnCanceled);
        //}
        //public static Task<T> NoThrowWhenCanceled<T>(this Task<T> continuation)
        //{
        //    return continuation.ContinueWith(
        //        ant => 
        //        {
        //            var ex = ant.Exception;
        //            return default(T);
        //        }, TaskContinuationOptions.OnlyOnCanceled);
        //}

        /// <summary>
        /// Ловит <see cref="OperationCanceledException"/> и <see cref="AggregateException"/> при условии, что
        /// все его <see cref="AggregateException.InnerExceptions"/> наследуются от 
        /// <see cref="OperationCanceledException"/>
        /// </summary>
        /// <param name="continuation"></param>
        /// <returns></returns>
        public static async Task CatchOperationCanceledExeption(this Task continuation)
        {
            try
            {
                await continuation;
            }
            catch (OperationCanceledException)
            {

            }
            catch (AggregateException ex)
            {
                if (!ex.InnerExceptions.All(e => typeof(OperationCanceledException).IsAssignableFrom(e.GetType())))
                {
                    throw;
                }
            }
        }

        public static async Task CatchAnyExeption(this Task continuation)
        {
            try
            {
                await continuation;
            }
            catch
            {

            }
        }
        public static async Task<T> CatchAnyExeptionOrDefault<T>(this Task<T> continuation)
        {
            try
            {
                return await continuation;
            }
            catch
            {
                return default;
            }
        }

        /// <summary>
        /// Ловит <see cref="OperationCanceledException"/> и <see cref="AggregateException"/> при условии, что
        /// все его <see cref="AggregateException.InnerExceptions"/> наследуются от 
        /// <see cref="OperationCanceledException"/>
        /// </summary>
        /// <param name="continuation"></param>
        /// <returns></returns>
        //public static async Task<T> CatchOperationCanceledExeption<T>(this Task<T> continuation)
        //{
        //    try
        //    {
        //        return await continuation;
        //    }
        //    catch (OperationCanceledException)
        //    {
                
        //    }
        //    catch (AggregateException ex)
        //    {
        //        if (!ex.InnerExceptions.All(e => typeof(OperationCanceledException).IsAssignableFrom(e.GetType())))
        //        {
        //            throw;
        //        }
        //    }

        //    return await Task.FromResult<T>(default(T));
        //}

        //public static async Task WhenCanceled(this Task continuation, Action action)
        //{
        //    await continuation.ContinueWith((ant) => { action(); }, TaskContinuationOptions.OnlyOnCanceled);
        //}

        //public static async Task<T> WhenCanceled<T>(this Task<T> continuation, Action action)
        //{
        //    return await continuation.ContinueWith((ant) => { action(); return default(T); }, 
        //        TaskContinuationOptions.OnlyOnCanceled);
        //}

        public static IDisposable Acquire(this SemaphoreSlim semaphore)
        {
            return semaphore.Acquire(CancellationToken.None);
        }
        public static IDisposable Acquire(this SemaphoreSlim semaphore, CancellationToken cancellation)
        {
            semaphore.Wait(cancellation);
            return new DisposingAction(() => semaphore.Release());
        }
        public static async Task<IDisposable> AcquireAsync
            (this SemaphoreSlim semaphore, CancellationToken cancellation)
        {
            await semaphore.WaitAsync(cancellation);
            return new DisposingAction(() => semaphore.Release());
        }
        public static async Task<IDisposable> AcquireAsync
            (this SemaphoreSlim semaphore)
        {
            return await semaphore.AcquireAsync(CancellationToken.None);
        }

        /// <summary>
        /// Makes use of <see cref="Task.WhenAll"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tasks"></param>
        /// <returns></returns>
        public static async Task WhenAll(this IEnumerable<Task> tasks)
        {
            await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Makes use of <see cref="Task.WaitAll"/>
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="tasks"></param>
        /// <returns></returns>
        public static void WaitAll(this IEnumerable<Task> tasks)
        {
            Task.WaitAll(tasks.ToArray());
        }

        public static Task<TOut> ThenDo<T, TOut>(this Task<T> action, Func<T, TOut> func)
        {
            return action.ContinueWith(t => func(t.Result), TaskContinuationOptions.OnlyOnRanToCompletion);
        }
        public static Task ThenDo<T>(this Task<T> task, Action<T> action)
        {
            return task.ContinueWith(t => action(t.Result), TaskContinuationOptions.OnlyOnRanToCompletion);
        }
    }
}
