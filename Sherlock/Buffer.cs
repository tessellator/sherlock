using System;
using System.Collections.Generic;
using System.Threading;

namespace Sherlock
{
    /// <summary>
    /// A base implementation of <see cref="T:IBuffer`1{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of value to store.</typeparam>
    public abstract class Buffer<T> : IBuffer<T>
    {
        private static readonly TimeSpan indefinite = new TimeSpan(0, 0, 0, 0, -1);

        private readonly object locker;
        private readonly Queue<T> queue;
        private readonly ManualResetEventSlim canReadEvent;
        private readonly ManualResetEventSlim canWriteEvent;
        private readonly ManualResetEventSlim disposedEvent;
        private bool disposed;

        /// <summary>
        /// Initializes the buffer.
        /// </summary>
        public Buffer()
        {
            locker = new object();
            queue = new Queue<T>();
            canWriteEvent = new ManualResetEventSlim(false);
            canReadEvent = new ManualResetEventSlim(false);
            disposedEvent = new ManualResetEventSlim(false);
        }

        /// <summary>
        /// Cleans up after the buffer.
        /// </summary>
        ~Buffer()
        {
            Dispose(false);
        }

        /// <summary>
        /// Disposes the buffer.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Disposes the buffer.
        /// </summary>
        /// <param name="disposing">
        /// A value indicating whether the method was called from 
        /// <see cref="M:Dispose"/>.
        /// </param>
        ///
        /// <remarks>
        /// Extended implementations should call this base implementation.
        /// </remarks>
        protected virtual void Dispose(bool disposing)
        {
            disposedEvent.Set();

            if (disposing && !disposed)
            {
                disposedEvent.Dispose();
                canWriteEvent.Dispose();
                canReadEvent.Dispose();
                disposed = true;
            }
        }

        /// <summary>
        /// Gets a value indicating whether the buffer is empty.
        /// </summary>
        /// <returns>A value indicating whether the buffer is empty.</returns>
        public bool IsEmpty()
        {
            lock (locker)
            {
                return queue.Count == 0;
            }
        }

        /// <summary>
        /// Gets a value indicating whether a new value may be put into the
        /// specified queue.
        /// </summary>
        /// <param name="queue">The queue to test.</param>
        /// <returns>A value indicating whether a new value may be put in the
        /// queue.</returns>
        ///
        /// <remarks>
        /// <p>This method is called in a thread-safe context with respect to
        /// the queue.</p>
        /// <p>This method should not throw an exception.</p>
        /// </remarks>
        protected virtual bool CanPut(Queue<T> queue)
        {
            return true;
        }

        /// <summary>
        /// Puts the specified item in the buffer.
        /// </summary>
        /// <param name="item">The item to put in the buffer.</param>
        ///
        /// <exception cref="ObjectDisposedException">
        /// The current buffer is disposed.
        /// </exception>
        ///
        /// <exception cref="InvalidOperationException">
        /// The put operation failed.
        /// </exception>
        public void Put(T item)
        {
            Put(item, indefinite);
        }

        /// <summary>
        /// Puts the specified item in the buffer, failing if the put 
        /// operation is not completed before a specified timeout passes.
        /// </summary>
        /// <param name="item">The item to put in the buffer.</param>
        /// <param name="timeout">The timeout after which the put fails.</param>
        ///
        /// <exception cref="ObjectDisposedException">
        /// The current buffer is disposed.
        /// </exception>
        ///
        /// <exception cref="InvalidOperationException">
        /// The put operation failed.
        /// </exception>
        public void Put(T item, TimeSpan timeout)
        {
            if (disposed)
                throw new ObjectDisposedException("The buffer has been disposed");

            if (!TryPut(item, timeout))
                throw new InvalidOperationException("The put operation failed");
        }

        /// <summary>
        /// Tries to put the specified item in the buffer.
        /// </summary>
        /// <param name="item">The item to put in the buffer.</param>
        /// <returns>A value indicating success.</returns>
        public bool TryPut(T item)
        {
            return TryPut(item, indefinite);
        }

        /// <summary>
        /// Tries to put the specified item in the buffer within the specified
        /// timeout period.
        /// </summary>
        /// <param name="item">The item to put in the buffer.</param>
        /// <param name="timeout">The timeout period.</param>
        /// <returns>A value indicating success.</returns>
        public bool TryPut(T item, TimeSpan timeout)
        {
            if (disposed || !CheckCanPut(timeout)) return false;

            var result = false;

            lock (locker)
            {
                result = Put(queue, item);
            }

            if (result)
                canReadEvent.Set();

            return result;
        }

        private bool CheckCanPut(TimeSpan timeout)
        {
            var canPut = false;

            lock (locker)
            {
                canPut = CanPut(queue);
            }

            return canPut || BlockUntilQueueIsPuttable(timeout);
        }

        private bool BlockUntilQueueIsPuttable(TimeSpan timeout)
        {
            canWriteEvent.Reset();
            var index = WaitHandle.WaitAny(new[] { canWriteEvent.WaitHandle, disposedEvent.WaitHandle }, timeout);
            return !(index == 1 || index == WaitHandle.WaitTimeout);
        }

        /// <summary>
        /// Puts the specified item into the specified queue.
        /// </summary>
        /// <param name="queue">
        /// The queue into which the item should be put.
        /// </param>
        /// <param name="item">
        /// The item to put into the queue.
        /// </param>
        /// <returns>
        /// A value indicating success.
        /// </returns>
        ///
        /// <remarks>
        /// <p>This method is called in a thread-safe context with respect to
        /// the queue.</p>
        ///
        /// <p>This method should not throw an exception.</p>
        ///
        /// <p>This method is only called if <see cref="M:CanPut"/> returns
        /// true.</p>
        ///
        /// <p>Implementors may manipulate the queue according to need.  For
        /// example, in a sliding buffer implentation the implementor may
        /// dequeue some number of items to reach a desired buffer size and then
        /// enqueue the specified item.</p>
        /// </remarks>
        protected abstract bool Put(Queue<T> queue, T item);

        /// <summary>
        /// Takes the first available value from the buffer.
        /// </summary>
        /// <returns>The first value from the buffer.</returns>
        ///
        /// <exception cref="ObjectDisposedException">
        /// The current buffer is disposed.
        /// </exception>
        ///
        /// <exception cref="InvalidOperationException">
        /// The take operation failed.
        /// </exception>
        public T Take()
        {
            return Take(indefinite);
        }

        /// <summary>
        /// Takes the first item from the buffer, failing if the take operation
        /// is not completed within the specified timeout period.
        /// </summary>
        /// <param name="timeout">The timeout period.</param>
        /// <returns>The first value from the buffer.</returns>
        ///
        /// <exception cref="ObjectDisposedException">
        /// The current buffer is disposed.
        /// </exception>
        ///
        /// <exception cref="InvalidOperationException">
        /// The put operation failed.
        /// </exception>
        public T Take(TimeSpan timeout)
        {
            T item;

            if (disposed)
                throw new ObjectDisposedException("The buffer has been disposed");

            if (!TryTake(timeout, out item))
                throw new InvalidOperationException("The take operation failed");

            return item;
        }

        /// <summary>
        /// Tries to take the first item from the buffer.
        /// </summary>
        /// <param name="item">The taken item.</param>
        /// <returns>A value indicating success.</returns>
        public bool TryTake(out T item)
        {
            return TryTake(indefinite, out item);
        }

        /// <summary>
        /// Tries to take the first item from the buffer within the timeout 
        /// period.
        /// </summary>
        /// <param name="timeout">The timeout period.</param>
        /// <param name="item">The taken item.</param>
        /// <returns>A value indicating success.</returns>
        public bool TryTake(TimeSpan timeout, out T item)
        {
            item = default(T);

            while (true)
            {
                if (disposed || !BlockUntilItemAvailable(timeout))
                    return false;

                lock (locker)
                {
                    if (queue.Count > 0)
                    {
                        item = queue.Dequeue();
                        this.canWriteEvent.Set();
                        return true;
                    }
                }
            }
        }

        private bool BlockUntilItemAvailable(TimeSpan timeout)
        {
            if (queue.Count == 0)
            {
                this.canReadEvent.Reset();
                var index = WaitHandle.WaitAny(new[] { canReadEvent.WaitHandle, disposedEvent.WaitHandle }, timeout);
                if (index == 1 || index == WaitHandle.WaitTimeout)
                    return false;
            }

            return true;
        }
    }
}
