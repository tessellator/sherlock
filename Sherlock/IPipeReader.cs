using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sherlock
{
   public interface IPipeReader<T> : IDisposable
   {
      bool Read(out T item);
      void Close();
      bool IsClosed { get; }
   }
}
