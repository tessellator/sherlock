using System;
using System.Threading;

using NUnit.Framework;

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
            int result = 0;
            producer = () =>
                {
                    for (int i = 0; i < 10; i++)
                        pipe.Writer.Write(i);

                    pipe.Writer.Close();
                };
            consumer = () =>
                {
                    int item;
                    while (pipe.Reader.Read(out item))
                    {
                        result += item;
                        Thread.Sleep(5);
                    }
                    doneEvent.Set();
                };

            ParallelThread.Invoke(producer, consumer);
            doneEvent.WaitOne();

            Assert.AreEqual(45, result);
        }

        [Test]
        public void Test_SlowProducer()
        {
            int result = 0;
            producer = () =>
                {
                    for (int i = 0; i < 10; i++)
                    {
                        pipe.Writer.Write(i);
                        Thread.Sleep(5);
                    }

                    pipe.Writer.Close();
                };
            consumer = () =>
                {
                    int item;

                    while (pipe.Reader.Read(out item))
                        result += item;

                    doneEvent.Set();
                };

            ParallelThread.Invoke(producer, consumer);
            doneEvent.WaitOne();

            Assert.AreEqual(45, result);
        }

        [Test]
        public void Test_ReaderClosedFirst()
        {
            int result = 0;
            producer = () =>
                {
                    for (int i = 0; i < 10; i++)
                        pipe.Writer.Write(i);

                    pipe.Writer.Close();
                    doneEvent.Set();
                };
            consumer = () =>
                {
                    int item;

                    for (int i = 0; i < 5; i++)
                    {
                        if (!pipe.Reader.Read(out item))
                            break;

                        result += item;
                    }

                    pipe.Reader.Close();
                };

            ParallelThread.Invoke(producer, consumer);
            doneEvent.WaitOne();

            Assert.AreEqual(10, result);
        }
    }
}
