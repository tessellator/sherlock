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

         reader.SetWriteCloseListener(writer);
         writer.SetReadCloseListener(reader);

         return new Pipe<T>(buffer, reader, writer);
      }

      static void reader_Closed(object sender, EventArgs e)
      {
         throw new NotImplementedException();
      }
   }

   class Pipe<T> : IPipe<T>
   {
      private readonly PipeReader<T> reader;
      private readonly PipeWriter<T> writer;
      private readonly IBuffer<T> buffer;

      public Pipe(IBuffer<T> buffer, PipeReader<T> reader, PipeWriter<T> writer)
      {
         this.reader = reader;
         this.writer = writer;
         this.buffer = buffer;

         reader.Closed += OnEndpointClosed;
         writer.Closed += OnEndpointClosed;
      }

      public IPipeReader<T> Reader
      {
         get { return reader; }
      }

      public IPipeWriter<T> Writer
      {
         get { return writer; }
      }

      private void OnEndpointClosed(object sender, EventArgs e)
      {
         if (reader.IsClosed && writer.IsClosed)
            buffer.Dispose();
      }


   }
}
