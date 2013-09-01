using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sherlock
{
   public interface IPipe<T>
   {
      IPipeReader<T> Reader { get; }
      IPipeWriter<T> Writer { get; }
   }
}
