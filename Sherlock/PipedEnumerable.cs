using System.Collections;
using System.Collections.Generic;

namespace Sherlock
{
    /// <summary>
    /// An <see cref="T:IEnumerable`1{T}"/> collection generated from the values
    /// read from an <see cref="T:IPipeReader`1{T}"/>.
    /// </summary>
    /// <typeparam name="T">The type of value to enumerate.</typeparam>
    public sealed class PipedEnumerable<T> : IEnumerable<T>
    {
        private readonly PipedEnumerator<T> enumerator;

        /// <summary>
        /// Initializes the collection with the specified reader.
        /// </summary>
        /// <param name="reader">The reader from which values will be read.</param>
        public PipedEnumerable(IPipeReader<T> reader)
        {
            enumerator = new PipedEnumerator<T>(reader);
        }

        /// <summary>
        /// Gets an enumerator over the collection.
        /// </summary>
        /// <returns>An enumerator over the collection.</returns>
        public IEnumerator<T> GetEnumerator()
        {
            return enumerator;
        }

        /// <summary>
        /// Gets an enumerator over the collection.
        /// </summary>
        /// <returns>An enumerator over the collection.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
