using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sherlock.Collections.Generic
{
    public interface IBuffer<T> : IDisposable
    {
        void Put(T item);
        T Take();
        void Put(TimeSpan timeout, T item);
        T Take(TimeSpan timeout);
        bool TryPut(T item);
        bool TryTake(out T item);
        bool TryPut(TimeSpan timeout, T item);
        bool TryTake(TimeSpan timeout, out T item);
    }
}
