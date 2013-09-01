using System;

namespace Sherlock
{
    /// <summary>
    /// A reader than can read values from a pipe.
    /// </summary>
    /// <typeparam name="T">The type of value to read.</typeparam>
    public interface IPipeReader<T> : IDisposable
    {
        /// <summary>
        /// Attempts to read a value from the pipe.
        /// </summary>
        /// <param name="item">The read value.</param>
        /// <returns>A value indicating success.</returns>
        bool Read(out T item);

        /// <summary>
        /// Closes the reader.
        /// </summary>
        void Close();

        /// <summary>
        /// Gets a value indicating whether the reader is closed.
        /// </summary>
        bool IsClosed { get; }
    }
}
