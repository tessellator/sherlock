using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sherlock.Collections.Generic
{
   public interface IPipeReader<T> : IDisposable
   {
      bool Read(out T item);
      void Close();
      bool IsClosed { get; }
   }
}
