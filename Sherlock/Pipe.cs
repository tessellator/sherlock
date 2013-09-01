using System;

namespace Sherlock
{
   /// <summary>
   /// Creates pipes.
   /// </summary>
   public static class Pipe
   {
      /// <summary>
      /// Opens a pipe that will never block on write operations.
      /// </summary>
      /// <typeparam name="T">
      /// The type of values to pass through the pipe.
      /// </typeparam>
      /// <returns>
      /// An open pipe.
      /// </returns>
      public static IPipe<T> Open<T>()
      {
         return Open(new UnboundedBuffer<T>());
      }

      /// <summary>
      /// Opens a pipe that will block on write operations when the pipe
      /// contains <paramref name="maxSize"/> values.
      /// </summary>
      /// <typeparam name="T">
      /// The type of values to pass through the pipe.
      /// </typeparam>
      /// <param name="maxSize">
      /// The maximum number of items allowed to be in the pipe before blocking
      /// on write operations.
      /// </param>
      /// <returns>An open pipe.</returns>
      public static IPipe<T> Open<T>(long maxSize)
      {
         return Open(new BoundedBuffer<T>(maxSize));
      }

      /// <summary>
      /// Opens a pipe with the specified buffer.
      /// </summary>
      /// <typeparam name="T">
      /// The type of values to pass through the pipe.
      /// </typeparam>
      /// <param name="buffer"></param>
      /// <returns>An open pipe that exhibits the blocking behavior specified by 
      /// <paramref name="buffer"/>.</returns>
      public static IPipe<T> Open<T>(IBuffer<T> buffer)
      {
         var reader = new PipeReader<T>(buffer);
         var writer = new PipeWriter<T>(buffer);

         reader.SetWriteCloseListener(writer);
         writer.SetReadCloseListener(reader);

         return new Pipe<T>(buffer, reader, writer);
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
