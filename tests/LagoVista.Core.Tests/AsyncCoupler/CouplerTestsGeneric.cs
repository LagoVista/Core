// --- BEGIN CODE INDEX META (do not edit) ---
// ContentHash: ebe441c17446acc666b31de9f9b93b050766fad1e78b2b0952145ad3d2193a4d
// IndexVersion: 2
// --- END CODE INDEX META ---
using LagoVista.Core.AsyncCoupler.Utils.Tests;
using LagoVista.Core.Utils;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Threading.Tasks;

namespace LagoVista.Core.AsyncCoupler.Tests
{
    [TestClass]
    public class CouplerTestsGeneric
    {
        AsyncCoupler<TestModelGenerics> _coupler;

        
        public CouplerTestsGeneric()
        {
            _coupler = new AsyncCoupler<TestModelGenerics>(new TestLogger(), new TestUsageMetrics("rpc", "rpc", "rpc") { Version = "N/A" });
        }


        public class TestModelGenerics
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }

        private void CompleteIt(string id, TestModelGenerics result, int ms = 250)
        {
            Task.Run(async () =>
            {
                await Task.Delay(ms);
                await _coupler.CompleteAsync(id, result);
            });
        }


        [TestMethod]
        public async Task Enqueue_Dequeue_Generic_Class_Simple_Valid()
        {
            var id = "C41DE0C9658143E1A4EB00ABFE638CAD";

            var testModel = new TestModelGenerics()
            {
                Id = "123456",
                Name = "Some test Data"
            };

            var start = DateTime.Now;

            var delay = 500;

            CompleteIt(id, testModel, delay);

            var result = await _coupler.WaitOnAsync(id, TimeSpan.FromSeconds(5));

            var delta = DateTime.Now - start;

            /* Just sanity check ot make sure that we waited for at least how long the process should be decoupled */
            Assert.IsTrue(delta.TotalMilliseconds > delay);
            Assert.IsNotNull(result);
            Assert.IsTrue(result.Successful);
            Assert.AreEqual(testModel.Id, result.Result.Id);
            Assert.AreEqual(testModel.Name, result.Result.Name);
            Assert.AreEqual(0, _coupler.ActiveSessions);
        }


        [TestMethod]
        public async Task Enqueue_Dequeue_Generic_Class_CorrelationId_DoesNotExistd()
        {
            var id = "C41DE0C9658143E1A4EB00ABFE638CAD";

            var testModel = new TestModelGenerics()
            {
                Id = "123456",
                Name = "Some test Data"
            };

            var result = await _coupler.CompleteAsync(id, testModel);

            Assert.IsNotNull(result);
            Assert.IsFalse(result.Successful);
            Assert.AreEqual($"Correlation id not found: {id}.", result.Errors[0].Message);
            Assert.AreEqual(0, _coupler.ActiveSessions);
        }

        [TestMethod]
        public async Task Enqueue_Dequeue_Generic_Class_Simple_Timeout_Failed()
        {
            var id = "C41DE0C9658143E1A4EB00ABFE638CAD";

            var testModel = new TestModelGenerics()
            {
                Id = "123456",
                Name = "Some test Data"
            };

            var result = await _coupler.WaitOnAsync(id, TimeSpan.FromMilliseconds(500));

            Assert.IsNotNull(result);
            Assert.IsFalse(result.Successful);
            Assert.AreEqual("Timeout waiting for response.", result.Errors[0].Message);
            Assert.AreEqual(0, _coupler.ActiveSessions);
        }


        [TestMethod]
        public async Task Enqueue_Dequeue_Generic_Class_Null_Failed()
        {
            var id = "C41DE0C9658143E1A4EB00ABFE638CAD";

            var testModel = new TestModelGenerics()
            {
                Id = "123456",
                Name = "Some test Data"
            };

            CompleteIt(id,null);

            var result = await _coupler.WaitOnAsync(id, TimeSpan.FromMilliseconds(1500));

            Assert.IsNotNull(result);
            Assert.IsFalse(result.Successful);
            Assert.AreEqual("Null Response From Completion Routine.", result.Errors[0].Message);
            Assert.AreEqual(0, _coupler.ActiveSessions);
        }
    }
}
