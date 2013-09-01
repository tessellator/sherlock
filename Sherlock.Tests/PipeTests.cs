using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NUnit.Framework;
using Sherlock;

namespace Sherlock.Tests
{
    [TestFixture]
    public class PipeTests
    {
        private IPipe<int> pipe;
        private Action producer;
        private Action consumer;
        private ManualResetEvent doneEvent;

        [SetUp]
        public void SetUp()
        {
            pipe = Pipe.Open<int>(2);
            doneEvent = new ManualResetEvent(false);
        }

        [TearDown]
        public void TearDown()
        {
            pipe.Reader.Dispose();
            pipe.Writer.Dispose();
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
                        pipe.Writer.Write(i);
                };
            consumer = () =>
                {
                    for (int i = 0; i < 10; i++)
                    {
                        int item;
                        pipe.Reader.Read(out item);
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
                        pipe.Writer.Write(i);
                };
            consumer = () =>
                {
                    for (int i = 0; i < 30; i++)
                    {
                        int item;
                        pipe.Reader.Read(out item);
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
                        pipe.Writer.Write(i);
                        Thread.Sleep(50);
                    }
                };
            consumer = () =>
                {
                    for (int i = 0; i < 10; i++)
                    {
                        int item;
                        pipe.Reader.Read(out item);
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
    }
}
