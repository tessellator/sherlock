using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sherlock.Collections.Generic
{
   public static class Pipe
   {
      public static IPipe<T> Open<T>() {
         var queue = new BlockingQueue<T>();
         return Open(queue);
      }

      public static IPipe<T> Open<T>(long maxSize)
      {
         var queue = new BlockingQueue<T>(maxSize);
         return Open(queue);
      }

      public static IPipe<T> Open<T>(IBuffer<T> buffer)
      {
         var queue = new BlockingQueue<T>(buffer);
         return Open(queue);
      }

      private static IPipe<T> Open<T>(BlockingQueue<T> queue)
      {
         var reader = new PipeReader<T>(queue);
         var writer = new PipeWriter<T>(queue);

         writer.SetReadCloseListener(reader);

         return new Pipe<T>(reader, writer);
      }
   }

   class Pipe<T> : IPipe<T>
   {
      private readonly IPipeReader<T> reader;
      private readonly IPipeWriter<T> writer;

      public Pipe(IPipeReader<T> reader, IPipeWriter<T> writer)
      {
         this.reader = reader;
         this.writer = writer;
      }

      public IPipeReader<T> Reader
      {
         get { return reader; }
      }

      public IPipeWriter<T> Writer
      {
         get { return writer; }
      }
   }
}
