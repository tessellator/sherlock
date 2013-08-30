using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Sherlock.Collections.Generic;

namespace Sherlock.Collections.Generic
{
    public class DroppingBuffer<T> : IBuffer<T>, IDisposable
    {
        private readonly object locker;
        private readonly long maxSize;
        private readonly Queue<T> queue;
        private readonly ManualResetEventSlim canRead;
        private readonly ManualResetEvent disposedEvent;
        private bool disposed;

        public DroppingBuffer(long maxSize)
        {
            this.maxSize = maxSize;
            this.locker = new object();
            this.queue = new Queue<T>();
            this.canRead = new ManualResetEventSlim(false);
            this.disposedEvent = new ManualResetEvent(false);
            this.disposed = false;
        }

        public void Put(T item)
        {
            Put(TimeOut.Indefinite, item);
        }

        public T Take()
        {
            return Take(TimeOut.Indefinite);
        }

        public void Put(TimeSpan timeout, T item)
        {
            if (!TryPut(timeout, item))
                throw new InvalidOperationException("Put operation failed");
        }

        public T Take(TimeSpan timeout)
        {
            T item;
            if (!TryTake(timeout, out item))
                throw new InvalidOperationException("Take operation failed");
            return item;
        }

        public bool TryPut(T item)
        {
            return TryPut(TimeOut.Indefinite, item);
        }

        public bool TryTake(out T item)
        {
            return TryTake(TimeOut.Indefinite, out item);
        }

        public bool TryPut(TimeSpan timeout, T item)
        {
            lock (locker)
            {
                if (this.maxSize > queue.Count)
                {
                    queue.Enqueue(item);
                    this.canRead.Set();
                    return true;
                }
                return false;
            }
        }

        public bool TryTake(TimeSpan timeout, out T item)
        {
            item = default(T);
            while (true)
            {
                if (queue.Count == 0)
                {
                    canRead.Reset();
                    var index = WaitHandle.WaitAny(new[] {canRead.WaitHandle, disposedEvent}, timeout);
                    if (index == 1 || index == WaitHandle.WaitTimeout)
                        return false;
                }
                lock (locker)
                {
                    if (queue.Count < this.maxSize)
                    {
                        item = queue.Dequeue();
                        return true;
                    }
                }
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (!disposed && disposing)
            {
                this.disposedEvent.Set();
                this.canRead.Dispose();
                this.disposedEvent.Dispose();
            }
            disposed = true;
        }
    }
}
