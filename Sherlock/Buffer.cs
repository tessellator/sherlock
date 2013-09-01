using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;

namespace Sherlock
{
   public abstract class Buffer<T> : IBuffer<T>
   {
      private static readonly TimeSpan indefinite = new TimeSpan(0, 0, 0, 0, -1);

      private readonly object locker;
      private readonly Queue<T> queue;
      private readonly ManualResetEventSlim canReadEvent;
      private readonly ManualResetEventSlim canWriteEvent;
      private readonly ManualResetEventSlim disposedEvent;
      private bool disposed;


      public Buffer()
      {
         locker = new object();
         queue = new Queue<T>();
         canWriteEvent = new ManualResetEventSlim(false);
         canReadEvent = new ManualResetEventSlim(false);
         disposedEvent = new ManualResetEventSlim(false);
      }

      ~Buffer()
      {
         Dispose(false);
      }

      public void Dispose()
      {
         Dispose(true);
         GC.SuppressFinalize(this);
      }

      protected virtual void Dispose(bool disposing)
      { 
         if (disposing && !disposed)
         {
            disposedEvent.Set();
            disposedEvent.Dispose();
            canWriteEvent.Dispose();
            canReadEvent.Dispose();
         }
         disposed = true;
      }

      public bool IsEmpty()
      {
         lock (locker)
         {
            return queue.Count == 0;
         }
      }

      protected virtual bool CanPut(Queue<T> queue)
      {
         return true;
      }

      public void Put(T item)
      {
         Put(indefinite, item);
      }

      public void Put(TimeSpan timeout, T item)
      {
         if (disposed)
            throw new ObjectDisposedException("The buffer has been disposed");

         if (!TryPut(timeout, item))
            throw new InvalidOperationException("The put operation failed");
      }

      public bool TryPut(T item)
      {
         return TryPut(indefinite, item);
      }

      public bool TryPut(TimeSpan timeout, T item)
      {
         if (disposed || !CheckCanPut(timeout)) return false;

         var result = false;
            
         lock (locker)
         {
            result = Put(queue, item);
         }

         if (result)
            canReadEvent.Set();

         return result;
      }

      private bool CheckCanPut(TimeSpan timeout)
      {
         var canPut = false;

         lock (locker)
         {
            canPut = CanPut(queue);
         }

         return canPut || BlockUntilQueueIsPuttable(timeout);
      }

      private bool BlockUntilQueueIsPuttable(TimeSpan timeout)
      {
         canWriteEvent.Reset();
         var index = WaitHandle.WaitAny(new[] { canWriteEvent.WaitHandle, disposedEvent.WaitHandle }, timeout);
         return !(index == 1 || index == WaitHandle.WaitTimeout);
      }

      protected abstract bool Put(Queue<T> queue, T item);

      public T Take()
      {
         return Take(indefinite);
      }

      public T Take(TimeSpan timeout)
      {
         T item;

         if (disposed)
            throw new ObjectDisposedException("The buffer has been disposed");

         if (!TryTake(timeout, out item))
            throw new InvalidOperationException("The take operation failed");

         return item;
      }

      public bool TryTake(out T item)
      {
         return TryTake(indefinite, out item);
      }

      public bool TryTake(TimeSpan timeout, out T item)
      {
         item = default(T);

         while (true)
         {
            if (disposed || !BlockUntilItemAvailable(timeout))
               return false;

            lock (locker)
            {
               if (queue.Count > 0)
               {
                  item = queue.Dequeue();
                  this.canWriteEvent.Set();
                  return true;
               }
            }
         }
      }

      private bool BlockUntilItemAvailable(TimeSpan timeout)
      {
         if (queue.Count == 0)
         {
            this.canReadEvent.Reset();
            var index = WaitHandle.WaitAny(new[] { canReadEvent.WaitHandle, disposedEvent.WaitHandle }, timeout);
            if (index == 1 || index == WaitHandle.WaitTimeout)
               return false;
         }

         return true;
      }
   }
}
