using System;
using System.Linq;
using System.Collections.Generic;
using Sherlock;
using System.Threading;

namespace Sherlock
{
    public class SlidingBuffer<T> : Buffer<T>
    {
        private readonly long maxSize;

        public SlidingBuffer(long maxSize)
        {
            if (maxSize < 1) 
                throw new ArgumentException("Max size must be greater than 0.");

            this.maxSize = maxSize;
        }

        protected override bool Put(Queue<T> queue, T item)
        {
           while (queue.Count >= maxSize)
              queue.Dequeue();

           queue.Enqueue(item);
           return true;
        }
    }
}
