﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sherlock
{
   public interface IPipeWriter<T> : IDisposable
   {
      void Write(T item);
      void Close();
      bool IsClosed { get; }
   }
}
