﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using NUnit.Framework;
using Sherlock.Collections.Generic;

namespace Sherlock.Tests.Generic
{
    [TestFixture]
    class PipedEnumerableTests
    {
        private IPipe<int> pipe;
        private PipedEnumerable<int> enumerable;
        private Action producer;

        [SetUp]
        public void SetUp()
        {
            pipe = Pipe.Open<int>(2);
            enumerable = new PipedEnumerable<int>(pipe.Reader);
            producer = () =>
                {
                    for (int i = 0; i < 10; i++)
                    {
                        pipe.Writer.Write(i);
                        Thread.Sleep(50);
                    }
                    pipe.Writer.Close();
                };

        }

        [Test]
        public void Test_GetEnumerator()
        {
            // Act
            ParallelThread.Invoke(producer);
            var result = enumerable.Aggregate((x, y) => x + y);

            // Assert
            Assert.AreEqual(45, result);
        }
    }
}