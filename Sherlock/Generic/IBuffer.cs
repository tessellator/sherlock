using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sherlock.Collections.Generic
{
   public interface IBuffer<T>
   {
      void Put(T item);
      T Take();
      bool TryPut(T item);
      bool TryTake(out T item);
      bool TryPut(TimeSpan timeout, T item);
      bool TryTake(TimeSpan timeout, out T item);
   }
}
