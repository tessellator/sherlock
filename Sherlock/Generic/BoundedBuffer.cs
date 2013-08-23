using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Sherlock.Collections.Generic
{
   public class BoundedBuffer<T> : IBuffer<T>
   {
      private readonly long maxSize;
      private readonly ConcurrentQueue<T> queue;
      private long emptyCount;
      private long fullCount;

      public BoundedBuffer()
         : this(long.MaxValue)
      {
      }

      public BoundedBuffer(long maxSize)
      {
         this.maxSize = maxSize;
         this.queue = new ConcurrentQueue<T>();
      }

      public void Put(T item)
      {
         Interlocked.Decrement(ref emptyCount);
         queue.Enqueue(item);
         Interlocked.Increment(ref fullCount);
      }

      public bool Take(out T item)
      {
         var result = queue.TryDequeue(out item);
         if (result)
         {
            Interlocked.Decrement(ref fullCount);
            Interlocked.Increment(ref emptyCount);
         }

         return result;
      }
   }
}
