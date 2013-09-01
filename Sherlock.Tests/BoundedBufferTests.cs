using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Sherlock;

namespace Sherlock.Tests
{
    [TestFixture]
    class BoundedBufferTests
    {
        private BoundedBuffer<int> boundedBuffer;

        [SetUp]
        public void SetUp()
        {
            boundedBuffer = new BoundedBuffer<int>(2);
        }

        [Test]
        [ExpectedException(typeof(ArgumentException))]
        public void Constructor_WithZeroMaxSize()
        {
            // Act
            new BoundedBuffer<int>(0);

            // Assert
            Assert.Fail("Failed to throw exception when size is not a positive integer.");
        }

        [Test]
        public void TryPut_WithFullBuffer()
        {
            // Assert
            var success = true;
            boundedBuffer.Put(5);
            boundedBuffer.Put(5);

            // Act
            success = boundedBuffer.TryPut(new TimeSpan(50), 5);

            // Assert
            Assert.IsFalse(success);
        }

        [Test]
        public void TryTake_WithEmptyBuffer()
        {
            // Arrange
            var success = true;
            int item;

            // Arrange
            success = boundedBuffer.TryTake(new TimeSpan(50), out item);

            // Assert
            Assert.IsFalse(success);
        }
        
        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Put_WithFullBuffer()
        {
            // Assert
            boundedBuffer.Put(5);
            boundedBuffer.Put(5);

            // Act
            boundedBuffer.Put(new TimeSpan(50), 5);

            // Assert
            Assert.Fail("Failed to throw exception when operation fails");
        }

        [Test]
        [ExpectedException(typeof(InvalidOperationException))]
        public void Take_WithEmptyBuffer()
        {
            // Act 
            boundedBuffer.Take(new TimeSpan(50));

            // Assert
            Assert.Fail("Failed to throw an exception when operation fails.");
        }

        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void Take_WhenDisposed()
        {
            // Arrange
            boundedBuffer.Dispose();

            // Act
            boundedBuffer.Take();

            // Assert
            Assert.Fail("Failed to raise exception when buffer is disposed.");
        }

        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void Put_WhenDisposed()
        {
            // Arrange
            boundedBuffer.Dispose();

            // Act
            boundedBuffer.Put(0);

            // Assert
            Assert.Fail("Failed to raise exception when buffer is disposed.");
        }
    }
}
