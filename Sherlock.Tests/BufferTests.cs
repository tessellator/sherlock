using System;
using System.Collections.Generic;

using NUnit.Framework;

namespace Sherlock.Tests
{
    [TestFixture]
    public class BufferTests
    {
        class BufferStub<T> : Buffer<T>
        {
            private bool canPut;
            private int canPutCalled;
            private int putCalled;
            private List<T> putCallValues;

            public void SetCanPut(bool canPut)
            {
                this.canPut = canPut;
                this.canPutCalled = 0;
                this.putCalled = 0;
                this.putCallValues = new List<T>();
            }

            protected override bool CanPut(Queue<T> queue)
            {
                canPutCalled++;
                return canPut;
            }

            protected override bool Put(Queue<T> queue, T item)
            {
                putCalled++;
                putCallValues.Add(item);
                queue.Enqueue(item);
                return true;
            }

            public int TimesCanPutCalled()
            {
                return canPutCalled;
            }

            public int TimesPutCalled()
            {
                return putCalled;
            }

            public bool PutCalledWith(T item)
            {
                return putCallValues.Contains(item);
            }
        }

        private BufferStub<int> buffer;
        private TimeSpan timeout;

        [SetUp]
        public void SetUp()
        {
            buffer = new BufferStub<int>();
            buffer.SetCanPut(true);

            timeout = new TimeSpan(50);
        }

        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void Put_AfterDispose_ThrowsException()
        {
            buffer.Dispose();
            buffer.Put(42, timeout);
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Put_OnFail_ThrowsException()
        {
            buffer.SetCanPut(false);
            buffer.Put(42, timeout);
        }

        [Test]
        public void TryPut_AfterDispose_ReturnsFalse()
        {
            buffer.Dispose();
            var success = buffer.TryPut(42);
            Assert.IsFalse(success);
        }

        [Test]
        public void TryPut_WhenCannotPut_ReturnsFalse()
        {
            buffer.SetCanPut(false);
            var success = buffer.TryPut(42, timeout);
            Assert.IsFalse(success);
        }

        [Test]
        public void TryPut_CallsImplCanPut()
        {
            buffer.TryPut(42, timeout);
            Assert.AreEqual(1, buffer.TimesCanPutCalled());
        }

        [Test]
        public void TryPut_CallsImplPut()
        {
            buffer.TryPut(42, timeout);
            Assert.AreEqual(1, buffer.TimesPutCalled());
            Assert.IsTrue(buffer.PutCalledWith(42));
        }

        [Test]
        public void TryPut_OnSuccess_ReturnsTrue()
        {
            var success = buffer.TryPut(42, timeout);
            Assert.IsTrue(success);
        }

        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void Take_AfterDispose_ThrowsException()
        {
            buffer.Dispose();
            buffer.Take();
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Take_AfterTryTakeFails_ThrowsException()
        {
            buffer.Take(timeout);
        }

        [Test]
        public void Take_ReturnsTakenValue()
        {
            buffer.Put(42);
            var value = buffer.Take();

            Assert.AreEqual(42, value);
        }

        [Test]
        public void TryTake_AfterDispose_ReturnsFalse()
        {
            int value;

            buffer.Dispose();
            var success = buffer.TryTake(out value);

            Assert.IsFalse(success);
        }

        [Test]
        public void TryTake_AfterTimeout_ReturnsFalse()
        {
            int value;

            var success = buffer.TryTake(timeout, out value);

            Assert.IsFalse(success);
        }

        [Test]
        public void TryTake_WhenSuccessful_ReturnsTrueAndSetsValue()
        {
            int value;

            buffer.Put(42);
            var success = buffer.TryTake(timeout, out value);

            Assert.IsTrue(success);
            Assert.AreEqual(42, value);
        }
    }
}
