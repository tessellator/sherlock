using System;
using System.Collections;
using System.Collections.Generic;

namespace Sherlock
{
    class PipedEnumerator<T> : IEnumerator<T>
    {
        private readonly IPipeReader<T> reader;
        private T current;

        public PipedEnumerator(IPipeReader<T> reader)
        {
            this.reader = reader;
            this.current = default(T);
        }

        public void Dispose()
        {
        }

        public T Current
        {
            get { return current; }
        }

        object IEnumerator.Current
        {
            get { return Current; }
        }

        public bool MoveNext()
        {
            return reader.Read(out current);
        }

        public void Reset()
        {
            throw new NotSupportedException();
        }
    }
}
