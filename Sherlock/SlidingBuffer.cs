using System;
using System.Collections.Generic;

namespace Sherlock
{
   public sealed class SlidingBuffer<T> : Buffer<T>
   {
      private readonly long maxSize;

      public SlidingBuffer(long maxSize)
      {
         if (maxSize < 1)
            throw new ArgumentException("Max size must be greater than 0.");

         this.maxSize = maxSize;
      }

      public long MaxSize
      {
         get { return maxSize; }
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
