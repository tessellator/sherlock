using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sherlock
{
   public static class Pipe
   {
      public static IPipe<T> Open<T>()
      {
         return Open(new UnboundedBuffer<T>());
      }

      public static IPipe<T> Open<T>(long maxSize)
      {
         return Open(new BoundedBuffer<T>(maxSize));
      }

      public static IPipe<T> Open<T>(IBuffer<T> buffer)
      {
         var reader = new PipeReader<T>(buffer);
         var writer = new PipeWriter<T>(buffer);

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
