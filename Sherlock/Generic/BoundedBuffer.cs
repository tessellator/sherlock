using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Sherlock.Collections.Generic
{
    public class BoundedBuffer<T> : IBuffer<T>
    {
        private readonly long maxSize;
        private readonly Queue<T> queue;
        private readonly object locker;
        private ManualResetEvent canWrite;

        public long MaxSize { get { return maxSize; } }

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
            this.canWrite = new ManualResetEvent(false);
        }

        public void Put(T item)
        {
            var success = false;
            while (!success)
            {
                if (queue.Length == this.maxSize)
                {
                    this.canWrite.Reset();
                    this.canWrite.WaitOne();
                }
                lock (locker)
                {
                    if (queue.Length < this.maxSize)
                    {
                        queue.Enqueue(item);
                        success = true;
                    }
                }
            }
        }

        public bool Take(out T item)
        {
            lock (locker)
            {
                if (queue.Length > 0) {
                    item = queue.Dequeue();
                    this.canWrite.Set();
                    return true;
                }
                item = default(T);
                return false;
            }
        }
    }
}
