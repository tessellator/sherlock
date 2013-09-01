using System;
using System.Collections.Generic;

namespace Sherlock
{
    /// <summary>
    /// A buffer that drops any new items while at a maximum size.
    /// </summary>
    /// <typeparam name="T">The type of value to store.</typeparam>
    ///
    /// <remarks>
    /// A dropping buffer never blocks a producer on write operations.
    /// </remarks>
    public sealed class DroppingBuffer<T> : Buffer<T>
    {
        private readonly long maxSize;

        /// <summary>
        /// Initializes the buffer with the specified maximum size.
        /// </summary>
        /// <param name="maxSize">The maximum size of the buffer.</param>
        ///
        /// <exception cref="ArgumentException">
        /// <paramref name="maxSize"/> is not a positive integer.
        /// </exception>
        public DroppingBuffer(long maxSize)
        {
            if (maxSize < 1)
                throw new ArgumentException("Max size must be greater than 0.");

            this.maxSize = maxSize;
        }

        /// <summary>
        /// Gets the maximum size of the buffer.
        /// </summary>
        public long MaxSize
        {
            get { return maxSize; }
        }

        /// <summary>
        /// Puts the specified item into the specified queue.
        /// </summary>
        /// <param name="queue">
        /// The queue into which the item should be put.
        /// </param>
        /// <param name="item">
        /// The item to put into the queue.
        /// </param>
        /// <returns>
        /// A value indicating whether the item was put into the queue.
        /// </returns>
        protected override bool Put(Queue<T> queue, T item)
        {
            if (queue.Count < maxSize)
            {
                queue.Enqueue(item);
                return true;
            }

            return false;
        }
    }
}
