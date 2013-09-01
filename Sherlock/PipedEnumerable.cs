using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sherlock
{
   public sealed class PipedEnumerable<T> : IEnumerable<T>
   {
      private readonly PipedEnumerator<T> enumerator;

      public PipedEnumerable(IPipeReader<T> reader)
      {
         enumerator = new PipedEnumerator<T>(reader);
      }

      public IEnumerator<T> GetEnumerator()
      {
         return enumerator;
      }

      System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
      {
         return GetEnumerator();
      }
   }
}
