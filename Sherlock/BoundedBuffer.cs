using System;
using System.Collections.Generic;

namespace Sherlock
{
    /// <summary>
    /// A buffer that blocks a producer when it reaches a maximum size.
    /// </summary>
    /// <typeparam name="T">The type of value to store.</typeparam>
    /// <remarks>
    /// A bounded buffer is good for ensuring that a producer does not flood a
    /// pipe but also that the consumer reads every value.
    /// </remarks>
    public class BoundedBuffer<T> : Buffer<T>
    {
        private readonly long maxSize;

        /// <summary>
        /// Initializes a bounded buffer with a maximum size of one hundred.
        /// </summary>
        public BoundedBuffer()
            : this(100)
        {
        }

        /// <summary>
        /// Initializes a bounded buffer with the specified maximum size.
        /// </summary>
        /// <param name="maxSize">The maximum size of the buffer.</param>
        ///
        /// <exception cref="ArgumentException">
        /// <paramref name="maxSize"/> is not a positive integer.
        /// </exception>
        public BoundedBuffer(long maxSize)
        {
            if (maxSize < 1)
                throw new ArgumentException("Max size must be greater than 0");

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
        /// Gets a value indicating whether a new value may be put into the
        /// specified queue.
        /// </summary>
        /// <param name="queue">The queue to test.</param>
        /// <returns>A value indicating whether the current buffer size
        /// exceeds the maximum size.</returns>
        protected sealed override bool CanPut(Queue<T> queue)
        {
            return queue.Count < maxSize;
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
        /// A value indicating success, always true.
        /// </returns>
        protected sealed override bool Put(Queue<T> queue, T item)
        {
            queue.Enqueue(item);
            return true;
        }
    }
}
