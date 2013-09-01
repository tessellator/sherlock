
namespace Sherlock.Collections.Generic
{
   public class UnboundedBuffer<T> : BoundedBuffer<T>
   {

      public UnboundedBuffer()
         : base(long.MaxValue)
      {
      }
   }
}
