using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Sherlock;

namespace Sherlock.Tests
{
    [TestFixture]
    public class SlidingBufferTests
    {
        private SlidingBuffer<int> slidingBuffer;

        [SetUp]
        public void SetUp()
        {
            slidingBuffer = new SlidingBuffer<int>(2);
        }

        [Test]
        public void Put_WhenBufferIsFull()
        {
            // Arrange
            slidingBuffer.Put(1);
            slidingBuffer.Put(2);
             
            // Act
            slidingBuffer.Put(3);

            // Assert
            Assert.AreEqual(2, slidingBuffer.Take(), "Failed to drop the first element in the buffer.");
        }
    }
}
