using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sherlock.Collections.Generic
{
   public interface IPipe<T>
   {
      IPipeReader<T> Reader { get; }
      IPipeWriter<T> Writer { get; }
   }
}
