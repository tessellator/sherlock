
namespace Sherlock
{
   public class UnboundedBuffer<T> : BoundedBuffer<T>
   {

      public UnboundedBuffer()
         : base(long.MaxValue)
      {
      }
   }
}
