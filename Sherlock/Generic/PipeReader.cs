using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sherlock.Collections.Generic
{
   class PipeReader<T> : IPipeReader<T>
   {
      private readonly BlockingQueue<T> queue;
      private bool isClosed;

      public event EventHandler Closed;

      public PipeReader(BlockingQueue<T> queue)
      {
         this.queue = queue;
      }

      public void Dispose()
      {
         Close();
         GC.SuppressFinalize(this);
      }

      public bool Read(out T item)
      {
         item = default(T);
         return !IsClosed && queue.Dequeue(out item);
      }

      public bool IsClosed
      {
         get { return isClosed; }
      }

      public void Close()
      {
         if (isClosed) return;

         isClosed = true;

         if (Closed != null)
            Closed(this, EventArgs.Empty);
      }
   }
}
