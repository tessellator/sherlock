using System;
using NUnit.Framework;

namespace Sherlock.Tests
{
    [TestFixture]
    public class BoundedBufferTests
    {
        private BoundedBuffer<int> boundedBuffer;
        private TimeSpan timeout;

        [SetUp]
        public void SetUp()
        {
            boundedBuffer = new BoundedBuffer<int>(2);
            timeout = new TimeSpan(50);
        }

        [Test]
        public void DefaultCtor_ProperlySetsMaxSize()
        {
            var buffer = new BoundedBuffer<int>();

            Assert.AreEqual(100, buffer.MaxSize);
        }

        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        [ExpectedException(typeof(ArgumentException))]
        public void Ctor_WithTooSmallMaxSize_ThrowsException(int maxSize)
        {
            new BoundedBuffer<int>(maxSize);
        }

        [Test]
        public void Ctor_ProperlySetsMaxSize()
        {
            Assert.AreEqual(2, boundedBuffer.MaxSize);
        }

        [Test]
        public void TryPut_WithFullBuffer()
        {
            boundedBuffer.Put(5);
            boundedBuffer.Put(5);

            var success = boundedBuffer.TryPut(5, timeout);

            Assert.IsFalse(success);
        }

        [Test]
        public void TryPut_WithEmptyBuffer()
        {
            var success = boundedBuffer.TryPut(42, timeout);

            Assert.IsTrue(success);
            Assert.AreEqual(42, boundedBuffer.Take());
        }
    }
}
