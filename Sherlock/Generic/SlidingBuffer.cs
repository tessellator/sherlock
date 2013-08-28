using System;
using System.Linq;
using System.Collections.Generic;

namespace Sherlock.Generic
{
    public class SlidingBuffer<T> : IBuffer<T>
    {
        private long maxSize;
        private Queue<T> queue;
        private object locker;

        public SlidingBuffer(long maxSize)
        {
            if (maxSize < 1) 
            {
                throw new ArgumentException("Max size must be greater than 0.");
            }

            this.maxSize = maxSize;
            this.queue = new Queue<T>();
            this.locker = new object();
        }

        public void Put(T item)
        {
            lock (locker)
            {
                while (queue.Length >= this.maxSize)
                {
                    queue.Dequeue();
                }
                queue.Enqueue(item);
            }

        }

        public override bool Take(out T item)
        {
            lock (locker)
            {
                if (queue.Length > 0)
                {
                    item = queue.Dequeue();
                    return true;
                }
                item = default(T);
                return false;
            }

        }
    }
}
