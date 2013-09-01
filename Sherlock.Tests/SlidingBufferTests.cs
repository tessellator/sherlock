using System;
using NUnit.Framework;

namespace Sherlock.Tests
{
    [TestFixture]
    public class SlidingBufferTests
    {
        private SlidingBuffer<int> buffer;
        private TimeSpan timeout;

        [SetUp]
        public void SetUp()
        {
            buffer = new SlidingBuffer<int>(3);
            buffer.Put(1);
            buffer.Put(2);
            buffer.Put(3);

            timeout = new TimeSpan(50);
        }

        [Test]
        public void Ctor_ProperlySetsMaxSize()
        {
            Assert.AreEqual(3, buffer.MaxSize);
        }

        [Test]
        [TestCase(0)]
        [TestCase(-1)]
        [ExpectedException(typeof(ArgumentException))]
        public void Ctor_WithTooSmallMaxSize_ThrowsException(int size)
        {
            new SlidingBuffer<int>(size);
        }

        [Test]
        public void Put_WhenQueueIsFull_ReturnsTrue()
        {
            var success = buffer.TryPut(4, timeout);
            Assert.IsTrue(success);
        }

        [Test]
        public void Put_WhenQueueIsFull_SlidesQueue()
        {
            buffer.TryPut(4, timeout);
            CollectionAssert.AreEquivalent(new[] { 2, 3, 4 }, buffer.GetAllValues());
        }
    }
}
