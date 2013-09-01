using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sherlock
{
   class PipedEnumerator<T> : IEnumerator<T>
   {
      private readonly IPipeReader<T> reader;
      private T current;

      public PipedEnumerator(IPipeReader<T> reader)
      {
         this.reader = reader;
         this.current = default(T);
      }

      public void Dispose()
      {
      }

      public T Current
      {
         get { return current; }
      }

      object System.Collections.IEnumerator.Current
      {
         get { return Current; }
      }

      public bool MoveNext()
      {
         return !reader.IsClosed && reader.Read(out current);
      }

      public void Reset()
      {
         throw new NotSupportedException();
      }
   }
}
