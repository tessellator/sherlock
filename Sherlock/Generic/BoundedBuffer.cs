using System;
using System.Collections.Generic;
using System.Threading;

namespace Sherlock.Collections.Generic
{
   public class BoundedBuffer<T> : Buffer<T>
   {
      private readonly long maxSize;

      public BoundedBuffer()
         : this(100)
      {
      }

      public BoundedBuffer(long maxSize)
      {
         if (maxSize < 1)
            throw new ArgumentException("Max size must be greater than 0");

         this.maxSize = maxSize; 
      }

      protected override bool CanPut(Queue<T> queue)
      {
         return queue.Count < maxSize;
      }

      protected override bool Put(Queue<T> queue, T item)
      {
         queue.Enqueue(item);
         return true;
      }
   }
}
