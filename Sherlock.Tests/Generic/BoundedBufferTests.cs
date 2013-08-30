using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using Sherlock.Collections.Generic;

namespace Sherlock.Collections.Generic.Tests
{

    [TestFixture]
    class BoundedBufferTests
    {
        private BoundedBuffer<int> boundedBuffer;
        private ManualResetEvent doneEvent;
        private Action producer;
        private Action consumer;

        [SetUp]
        public void SetUp()
        {
            doneEvent = new ManualResetEvent(false);
            boundedBuffer = new BoundedBuffer<int>(2);

        }

        [TearDown]
        public void TearDown()
        {
            boundedBuffer.Dispose();
            doneEvent.Dispose();
        }

        [Test]
        public void Test_SlowConsumer()
        {
            // Assert
            int result = 0;
            producer = () =>
                {
                    for (int i = 0; i < 10; i++)
                        boundedBuffer.Put(i);
                };
            consumer = () =>
                {
                    for (int i = 0; i < 10; i++)
                    {
                        int item = boundedBuffer.Take();
                        result += item;
                        Thread.Sleep(50);
                    }
                    doneEvent.Set();
                };

            // Act
            ParallelThread.Invoke(producer, consumer);
            doneEvent.WaitOne();

            // Assert
            Assert.AreEqual(45, result);
        }
        
        [Test]
        public void Test_ManyProducers()
        {
            // Assert
            int result = 0;
            producer = () =>
                {
                    for (int i = 0; i < 10; i++)
                        boundedBuffer.Put(i);
                };
            consumer = () =>
                {
                    for (int i = 0; i < 30; i++)
                    {
                        int item = boundedBuffer.Take();
                        result += item;
                    }
                    doneEvent.Set();
                };

            // Act
            ParallelThread.Invoke(producer, producer, producer, consumer);
            doneEvent.WaitOne();

            // Assert
            Assert.AreEqual(45 * 3, result);
        }

        [Test]
        public void Test_SlowProducer()
        {
            // Assert
            int result = 0;
            producer = () =>
                {
                    for (int i = 0; i < 10; i++)
                    {
                        boundedBuffer.Put(i);
                        Thread.Sleep(50);
                    }
                };
            consumer = () =>
                {
                    for (int i = 0; i < 10; i++)
                    {
                        int item = boundedBuffer.Take();
                        result += item;
                    }
                    doneEvent.Set();
                };

            // Act
            ParallelThread.Invoke(producer, consumer);
            doneEvent.WaitOne();

            // Assert
            Assert.AreEqual(45, result);
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
        public void TryTake_WhenDisposed()
        {
            // Arrange
            boundedBuffer.Dispose();

            // Act
            int item;
            boundedBuffer.TryTake(out item);

            // Assert
            Assert.Fail("Failed to raise exception when buffer is disposed.");
        }

        [Test]
        [ExpectedException(typeof(ObjectDisposedException))]
        public void TryPut_WhenDisposed()
        {
            // Arrange
            boundedBuffer.Dispose();

            // Act
            boundedBuffer.TryPut(0);

            // Assert
            Assert.Fail("Failed to raise exception when buffer is disposed.");
        }

        /// <summary>
        /// Since Parallel.Invoke does not guarantee the actions will
        /// run on separate threads, use this helper class. 
        /// </summary>
        private static class ParallelThread
        {
            public static void Invoke(params Action[] actions)
            {
                foreach (var action in actions)
                {
                    var thread = new Thread(new ThreadStart(action));
                    thread.Start();
                }
            }
        }
    }
}
