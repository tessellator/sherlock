using System;

namespace Sherlock
{
    class PipeReader<T> : IPipeReader<T>
    {
        private readonly IBuffer<T> buffer;
        private bool isClosed;
        private bool readyToClose;

        public event EventHandler Closed;

        public PipeReader(IBuffer<T> buffer)
        {
            this.buffer = buffer;
        }

        public void Dispose()
        {
            Close();
            GC.SuppressFinalize(this);
        }

        public bool Read(out T item)
        {
            if (buffer.IsEmpty() && readyToClose)
                Close();

            item = default(T);
            return !IsClosed && buffer.TryTake(out item);
        }

        public bool IsClosed
        {
            get { return isClosed; }
        }

        public void Close()
        {
            if (isClosed) return;

            isClosed = true;

            if (Closed != null)
                Closed(this, EventArgs.Empty);
        }

        public void SetWriteCloseListener(PipeWriter<T> writer)
        {
            writer.Closed += (o, e) =>
            {
                readyToClose = true;
                if (buffer.IsEmpty())
                    Close();
            };
        }
    }
}
