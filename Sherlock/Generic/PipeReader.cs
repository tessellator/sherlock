using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sherlock.Collections.Generic
{
   class PipeReader<T> : IPipeReader<T>
   {
      private readonly IBuffer<T> buffer;
      private bool isClosed;

      public event EventHandler Closed;

      public PipeReader(IBuffer<T> buffer)
      {
         this.buffer = buffer;
      }

      public void Dispose()
      {
         Close();
         GC.SuppressFinalize(this);
      }

      public bool Read(out T item)
      {
         item = default(T);
         return !IsClosed && buffer.TryTake(out item);
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
