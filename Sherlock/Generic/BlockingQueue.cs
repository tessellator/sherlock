using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sherlock.Collections.Generic
{
   public class BlockingQueue<T>
   {
      private readonly IBuffer<T> buffer;

      public BlockingQueue() 
         : this(new BoundedBuffer<T>())
      {
      }

      public BlockingQueue(long maxSize) 
         : this(new BoundedBuffer<T>(maxSize))
      {
      }

      public BlockingQueue(IBuffer<T> buffer)
      {
         this.buffer = buffer;
      }

      public void Enqueue(T item)
      {
         buffer.Put(item);
      }

      public bool Dequeue(out T item)
      {
         return buffer.Take(out item);
      }
   }
}
