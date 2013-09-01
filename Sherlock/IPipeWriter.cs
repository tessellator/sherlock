using System;

namespace Sherlock
{
    /// <summary>
    /// A writer than can write values to a pipe.
    /// </summary>
    /// <typeparam name="T">The type of value to write.</typeparam>
    public interface IPipeWriter<T> : IDisposable
    {
        /// <summary>
        /// Writes the specified value to the pipe.
        /// </summary>
        /// <param name="item">The item to put into the pipe.</param>
        /// <returns>A value indicating success.</returns>
        bool Write(T item);

        /// <summary>
        /// Closes the writer.
        /// </summary>
        void Close();

        /// <summary>
        /// Gets a value indicating whether the writer is closed.
        /// </summary>
        bool IsClosed { get; }
    }
}
