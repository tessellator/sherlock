using System;

namespace Sherlock
{
    /// <summary>
    /// Represents a value buffer.
    /// </summary>
    /// <typeparam name="T">
    /// The type of values to store.
    /// </typeparam>
    public interface IBuffer<T> : IDisposable
    {
        /// <summary>
        /// Puts the specified item in the buffer.
        /// </summary>
        /// <param name="item">The item to put in the buffer.</param>
        ///
        /// <exception cref="ObjectDisposedException">
        /// The current buffer is disposed.
        /// </exception>
        ///
        /// <exception cref="InvalidOperationException">
        /// The put operation failed.
        /// </exception>
        void Put(T item);

        /// <summary>
        /// Puts the specified item in the buffer, failing if the put 
        /// operation is not completed before a specified timeout passes.
        /// </summary>
        /// <param name="item">The item to put in the buffer.</param>
        /// <param name="timeout">The timeout after which the put fails.</param>
        ///
        /// <exception cref="ObjectDisposedException">
        /// The current buffer is disposed.
        /// </exception>
        ///
        /// <exception cref="InvalidOperationException">
        /// The put operation failed.
        /// </exception>
        void Put(T item, TimeSpan timeout);

        /// <summary>
        /// Tries to put the specified item in the buffer.
        /// </summary>
        /// <param name="item">The item to put in the buffer.</param>
        /// <returns>A value indicating success.</returns>
        bool TryPut(T item);

        /// <summary>
        /// Tries to put the specified item in the buffer within the specified
        /// timeout period.
        /// </summary>
        /// <param name="item">The item to put in the buffer.</param>
        /// <param name="timeout">The timeout period.</param>
        /// <returns>A value indicating success.</returns>
        bool TryPut(T item, TimeSpan timeout);

        /// <summary>
        /// Takes the first available value from the buffer.
        /// </summary>
        /// <returns>The first value from the buffer.</returns>
        ///
        /// <exception cref="ObjectDisposedException">
        /// The current buffer is disposed.
        /// </exception>
        ///
        /// <exception cref="InvalidOperationException">
        /// The take operation failed.
        /// </exception>
        T Take();

        /// <summary>
        /// Takes the first item from the buffer, failing if the take operation
        /// is not completed within the specified timeout period.
        /// </summary>
        /// <param name="timeout">The timeout period.</param>
        /// <returns>The first value from the buffer.</returns>
        ///
        /// <exception cref="ObjectDisposedException">
        /// The current buffer is disposed.
        /// </exception>
        ///
        /// <exception cref="InvalidOperationException">
        /// The put operation failed.
        /// </exception>
        T Take(TimeSpan timeout);

        /// <summary>
        /// Tries to take the first item from the buffer.
        /// </summary>
        /// <param name="item">The taken item.</param>
        /// <returns>A value indicating success.</returns>
        bool TryTake(out T item);

        /// <summary>
        /// Tries to take the first item from the buffer within the timeout 
        /// period.
        /// </summary>
        /// <param name="timeout">The timeout period.</param>
        /// <param name="item">The taken item.</param>
        /// <returns>A value indicating success.</returns>
        bool TryTake(TimeSpan timeout, out T item);

        /// <summary>
        /// Gets a value indicating whether the buffer is empty.
        /// </summary>
        /// <returns>A value indicating whether the buffer is empty.</returns>
        bool IsEmpty();
    }
}
