
namespace Sherlock
{
   public sealed class UnboundedBuffer<T> : BoundedBuffer<T>
   {

      public UnboundedBuffer()
         : base(long.MaxValue)
      {
      }
   }
}
