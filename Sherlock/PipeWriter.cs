using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sherlock
{
    class PipeWriter<T> : IPipeWriter<T>
    {
        private readonly IBuffer<T> buffer;
        private bool isClosed;

        public event EventHandler Closed;

        public PipeWriter(IBuffer<T> buffer)
        {
            this.buffer = buffer;
        }

        public void Dispose()
        {
            Close();
            GC.SuppressFinalize(this);
        }

        public void Write(T item)
        {
            if (IsClosed)
                throw new PipeClosedException();

            buffer.Put(item);
        }

        public void Close()
        {
            if (isClosed) return;

            isClosed = true;

            if (Closed != null)
                Closed(this, EventArgs.Empty);
        }

        public bool IsClosed
        {
            get { return isClosed; }
        }

        internal void SetReadCloseListener(PipeReader<T> reader)
        {
            reader.Closed += (o, e) => Close();
        }
    }
}
