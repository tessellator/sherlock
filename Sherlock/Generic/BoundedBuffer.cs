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
        private readonly ManualResetEvent canWriteEvent;
        private readonly ManualResetEvent canReadEvent;
        private readonly ManualResetEvent closedEvent;
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
            this.canWriteEvent = new ManualResetEvent(false);
            this.canReadEvent = new ManualResetEvent(false);
            this.closedEvent = new ManualResetEvent(false);
        }

        public void Put(T item)
        {
            var success = false;
            while (!success)
            {
                if (queue.Count == this.maxSize)
                {
                    this.canWriteEvent.Reset();
                    this.canWriteEvent.WaitOne();
                    var index = WaitHandle.WaitAny(new WaitHandle[] {this.canWriteEvent, closedEvent});
                    if (index == 1)
                        break;
                }
                lock (locker)
                {
                    if (queue.Count < this.maxSize)
                    {
                        queue.Enqueue(item);
                        success = true;
                        this.canReadEvent.Set();
                    }
                }
            }
        }

        public bool TryTake(out T item)
        {
            item = default(T);
            while (true)
            {
                if (queue.Count == 0)
                {
                    this.canReadEvent.Reset();
                    this.canReadEvent.WaitOne();
                    var index = WaitHandle.WaitAny(new WaitHandle[] {this.canReadEvent, closedEvent});
                    if (index == 1)
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
                closedEvent.Set();
                closedEvent.Dispose();
                canWriteEvent.Dispose();
                canReadEvent.Dispose();
            }
        }
    }
}
