using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sherlock.Collections.Generic
{
   class PipeWriter<T> : IPipeWriter<T>
   {
      private readonly BlockingQueue<T> queue;
      private bool isClosed;

      public event EventHandler Closed;

      public PipeWriter(BlockingQueue<T> queue)
      {
         this.queue = queue;
      }

      public void Dispose()
      {
         Close();
         GC.SuppressFinalize(this);
      }

      public void Write(T item)
      {
         if (IsClosed)
            throw new PipeClosedException();

         queue.Enqueue(item);
      }

      public void Close()
      {
         if (isClosed) return;

         isClosed = true;

         if (Closed != null)
            Closed(this, EventArgs.Empty);
      }

      public bool IsClosed
      {
         get { return isClosed; }
      }

      internal void SetReadCloseListener(PipeReader<T> reader)
      {
         reader.Closed += (o, e) => Close();
      }
   }
}
