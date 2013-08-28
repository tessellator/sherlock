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

      public BoundedBuffer()
         : this(100)
      {
      }

      public BoundedBuffer(long maxSize)
      {
         this.maxSize = maxSize;
         this.queue = new Queue<T>();
         this.locker = new object();
      }

      public void Put(T item)
      {
         lock (locker)
         {
            queue.Enqueue(item);
         }
      }

      public bool Take(out T item)
      {
         item = queue.Dequeue(item);
         return item;
      }
   }
}
