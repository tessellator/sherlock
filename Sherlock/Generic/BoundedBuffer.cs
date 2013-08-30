using System;
using System.Collections.Generic;
using System.Threading;

namespace Sherlock.Collections.Generic
{
    public class BoundedBuffer<T> : IBuffer<T>, IDisposable
    {
        private readonly long maxSize;
        private readonly Queue<T> queue;
        private readonly object locker;
        private readonly ManualResetEventSlim canWriteEvent;
        private readonly ManualResetEventSlim canReadEvent;
        private readonly ManualResetEvent disposedEvent;
        private bool disposed;

        public BoundedBuffer()
            : this(100)
        {
        }

        public BoundedBuffer(long maxSize)
        {
            if (maxSize < 1)
            {
                throw new ArgumentException("Max size must be greater than 0");
            }
            this.maxSize = maxSize;
            this.queue = new Queue<T>();
            this.locker = new object();
            this.canWriteEvent = new ManualResetEventSlim(false);
            this.canReadEvent = new ManualResetEventSlim(false);
            this.disposedEvent = new ManualResetEvent(false);
        }

        public void Put(T item)
        {
            Put(TimeOut.Indefinite, item);
        }

        public T Take()
        {
            return Take(TimeOut.Indefinite);
        }

        public bool TryPut(TimeSpan timeout, T item)
        {
            if (disposed)
                throw new ObjectDisposedException("The bounded buffer has been disposed");

            while (true)
            {
                if (queue.Count == this.maxSize)
                {
                    this.canWriteEvent.Reset();
                    var index = WaitHandle.WaitAny(new[] { this.canWriteEvent.WaitHandle, disposedEvent }, timeout);
                    if (index == 1 || index == WaitHandle.WaitTimeout)
                        return false;
                }
                lock (locker)
                {
                    if (queue.Count < this.maxSize)
                    {
                        queue.Enqueue(item);
                        this.canReadEvent.Set();
                        return true;
                    }
                }
            }
        }

        public bool TryTake(TimeSpan timeout, out T item)
        {
            if (disposed)
                throw new ObjectDisposedException("The bounded buffer has been disposed");

            item = default(T);
            while (true)
            {
                if (queue.Count == 0)
                {
                    this.canReadEvent.Reset();
                    var index = WaitHandle.WaitAny(new[] { this.canReadEvent.WaitHandle, disposedEvent }, timeout);
                    if (index == 1 || index == WaitHandle.WaitTimeout)
                        return false;
                }
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

        ~BoundedBuffer()
        {
            Dispose(false);
        }


        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void Dispose(bool disposing)
        {
            if (disposing && !disposed)
            {
                disposedEvent.Set();
                disposedEvent.Dispose();
                canWriteEvent.Dispose();
                canReadEvent.Dispose();
            }
            disposed = true;
        }

        public bool TryPut(T item)
        {
            return TryPut(TimeOut.Indefinite, item);
        }

        public bool TryTake(out T item)
        {
            return TryTake(TimeOut.Indefinite, out item);
        }

        public void Put(TimeSpan timeout, T item)
        {
           if (!TryPut(timeout, item))
              throw new InvalidOperationException("The put operation failed");

        }

        public T Take(TimeSpan timeout)
        {
            T item;
            if (!TryTake(timeout, out item))
                throw new InvalidOperationException("The take operation failed");
            return item;
        }
    }
}
