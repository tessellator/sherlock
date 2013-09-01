using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Sherlock;

namespace Sherlock
{
    public class DroppingBuffer<T> : Buffer<T>
    {
        private readonly long maxSize;

        public DroppingBuffer(long maxSize)
        {
            this.maxSize = maxSize;
        }

        protected override bool Put(Queue<T> queue, T item)
        {
           if (queue.Count < maxSize)
           {
              queue.Enqueue(item);
              return true;
           }

           return false;
        }
    }
}
