using System;
using System.Linq;
using System.Collections.Generic;
using Sherlock.Collections.Generic;
using System.Threading;

namespace Sherlock.Generic
{
    public class SlidingBuffer<T> : IBuffer<T>, IDisposable
    {
        private readonly long maxSize;
        private readonly Queue<T> queue;
        private readonly object locker;
        private bool disposed;
        private readonly ManualResetEvent canReadEvent;
        private readonly ManualResetEvent disposedEvent;

        public SlidingBuffer(long maxSize)
        {
            if (maxSize < 1) 
            {
                throw new ArgumentException("Max size must be greater than 0.");
            }

            this.maxSize = maxSize;
            this.queue = new Queue<T>();
            this.locker = new object();
            this.disposed = false;
            this.canReadEvent = new ManualResetEvent(false);
            this.disposedEvent = new ManualResetEvent(false);
        }

        public bool TryPut(TimeSpan timeout, T item)
        {
            if (disposed)
                return false;

            lock (locker)
            {
                while (queue.Count >= this.maxSize)
                {
                    queue.Dequeue();
                }
                queue.Enqueue(item);
                this.canReadEvent.Set();
                return true;
            }
        }

        public bool TryTake(TimeSpan timeout, out T item)
        {
            item = default(T);
            while (true)
            {
                if (queue.Count == 0)
                {
                    this.canReadEvent.Reset();
                    var index = WaitHandle.WaitAny(new WaitHandle[] {this.canReadEvent, this.disposedEvent}, timeout);

                    if (index == 1 || index == WaitHandle.WaitTimeout)
                        return false;
                }
                lock (locker)
                {
                    if (queue.Count > 0)
                    {
                        item = queue.Dequeue();
                        return true;
                    }
                }
            }
        }

        public T Take()
        {
            T item;
            if (!TryTake(new TimeSpan(-1), out item))
                throw new InvalidOperationException("The take operation failed");
            return item;
        }

        public void Put(T item)
        {
            if (!TryPut(new TimeSpan(-1), item))
                throw new InvalidOperationException("The put operation failed");

        }

        public bool TryPut(T item)
        {
            return TryPut(new TimeSpan(-1), item);
        }

        public bool TryTake(out T item)
        {
            return TryTake(new TimeSpan(-1), out item);
        }

        ~SlidingBuffer()
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
            if (!disposed && disposing)
            {
                this.disposedEvent.Set();
                this.canReadEvent.Dispose();
                this.disposedEvent.Dispose();
            }
            disposed = true;
        }
    }
}
