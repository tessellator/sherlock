using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sherlock.Tests
{
   public static class BufferExtensions
   {
      public static IList<T> GetAllValues<T>(this IBuffer<T> buffer)
      {
         var timeout = new TimeSpan(10);
         var list = new List<T>();
         T item;

         while (buffer.TryTake(timeout, out item))
            list.Add(item);

         return list;
      }
   }
}
