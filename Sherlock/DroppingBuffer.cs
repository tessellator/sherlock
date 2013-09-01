using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Sherlock;

namespace Sherlock
{
   public sealed class DroppingBuffer<T> : Buffer<T>
   {
      private readonly long maxSize;

      public DroppingBuffer(long maxSize)
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
         if (queue.Count < maxSize)
         {
            queue.Enqueue(item);
            return true;
         }

         return false;
      }
   }
}
