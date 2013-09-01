
namespace Sherlock
{
    /// <summary>
    /// An unbounded buffer.
    /// </summary>
    /// <typeparam name="T">The type of value to store.</typeparam>
    public sealed class UnboundedBuffer<T> : BoundedBuffer<T>
    {
        /// <summary>
        /// Initializes the buffer.
        /// </summary>
        public UnboundedBuffer()
            : base(long.MaxValue)
        {
        }
    }
}
