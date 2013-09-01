using System;
using NUnit.Framework;

namespace Sherlock.Tests
{
    [TestFixture]
    public class DroppingBufferTests
    {
        private DroppingBuffer<int> buffer;

        [SetUp]
        public void SetUp()
        {
            buffer = new DroppingBuffer<int>(3);
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
            new DroppingBuffer<int>(size);
        }

        [Test]
        public void Put_WhenQueueIsFull_ReturnsFalse()
        {
            buffer.Put(1);
            buffer.Put(2);
            buffer.Put(3);

            var success = buffer.TryPut(4);

            Assert.IsFalse(success);
        }

        [Test]
        public void Put_WhenQueueIsFull_DoesNotChangeQueue()
        {
            buffer.Put(1);
            buffer.Put(2);
            buffer.Put(3);

            buffer.TryPut(4);

            CollectionAssert.AreEquivalent(new[] { 1, 2, 3 }, buffer.GetAllValues());
        }
    }
}
