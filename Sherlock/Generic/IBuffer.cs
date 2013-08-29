using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sherlock.Collections.Generic
{
   public interface IBuffer<T>
   {
      void Put(T item);
      bool TryTake(out T item);
   }
}
