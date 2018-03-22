using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Dataflow;

namespace Tests
{
    [TestClass]
    public class DataflowTests
    {
        [TestMethod]
        public void TestEnumerableToBufferBlockWithTransformAndAsEnumerable()
        {
            var items = Enumerable
                            .Range(1, 10)
                            .ToBufferBlock()
                            .Transform(i => i * 2)
                            .AsEnumerable()
                            .ToList();

            Assert.AreEqual(10, items.Count);
            Assert.AreEqual(110, items.Sum());
        }

        [TestMethod]
        public void TestEnumerableToBufferBlockWithTransformAndAsEnumerableAsync()
        {
            var items = Enumerable
                            .Range(1, 10)
                            .ToBufferBlock()
                            .Transform(async i => await Task.FromResult(i * 2).ConfigureAwait(false))
                            .AsEnumerable()
                            .ToList();

            Assert.AreEqual(10, items.Count);
            Assert.AreEqual(110, items.Sum());
        }

        [TestMethod]
        public void TestEnumerableToBufferBlockWithAction()
        {
            var sum = 0;

            Enumerable
                .Range(1, 10)
                .ToBufferBlock()
                .Action(i => sum += i)
                .Completion.Wait();

            Assert.AreEqual(55, sum);
        }

        [TestMethod]
        public void TestEnumerableToBufferBlockWithActionAsync()
        {
            var sum = 0;

            Enumerable
                .Range(1, 10)
                .ToBufferBlock()
                .Action(async i => { sum += i; await Task.CompletedTask.ConfigureAwait(false); })
                .Completion.Wait();

            Assert.AreEqual(55, sum);
        }

        [TestMethod]
        public void TestEnumerableToBufferBlockWithBatchOf3()
        {
            var batches = Enumerable
                            .Range(1, 10)
                            .ToBufferBlock()
                            .Batch(3)
                            .AsEnumerable()
                            .ToList();

            Assert.AreEqual(4, batches.Count);
            Assert.AreEqual(3, batches[0].Length);
            Assert.AreEqual(3, batches[1].Length);
            Assert.AreEqual(3, batches[2].Length);
            Assert.AreEqual(1, batches[3].Length);
        }

        [TestMethod]
        public void TestEnumerableBufferBlockCompletion()
        {
            var complete = false;

            var items = Enumerable
                        .Range(1, 10)
                        .ToBufferBlock()
                        .OnCompletion(_ => complete = true)
                        .AsEnumerable()
                        .ToList();

            Assert.AreEqual(10, items.Count);

            // A delay is needed as this is sync and the completion happens async
            Task.Delay(500).Wait();

            Assert.IsTrue(complete);
        }
    }
}
