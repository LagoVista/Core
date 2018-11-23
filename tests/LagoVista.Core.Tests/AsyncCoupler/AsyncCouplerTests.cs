using LagoVista.Core.AsyncCoupler.Utils.Tests;
using LagoVista.Core.Utils;
using System;
using System.Threading.Tasks;
using Xunit;

namespace LagoVista.Core.AsyncCoupler.Tests
{
    public class AsyncCouplerTests
    {
        Core.Utils.AsyncCoupler _coupler;

        public AsyncCouplerTests()
        {
            _coupler = new Core.Utils.AsyncCoupler(new TestLogger(), new TestUsageMetrics("rpc", "rpc", "rpc") { Version = "N/A" });
        }

        public class TestModel
        {
            public string Id { get; set; }
            public string Name { get; set; }
        }

        public class WrongObject
        {
            public string SomeOtherField { get; set; }
        }

        private void CompleteIt<T>(string id, T result, int ms = 250)
        {
            Task.Run(async () =>
            {
                await Task.Delay(ms);
                await _coupler.CompleteAsync<T>(id, result);
            });
        }

        [Fact]
        public async Task Enqueue_Dequeue_Simple_Valid()
        {
            var id = "C41DE0C9658143E1A4EB00ABFE638CAD";

            var testModel = new TestModel()
            {
                Id = "123456",
                Name = "Some test Data"
            };

            var start = DateTime.Now;

            var delay = 500;

            CompleteIt(id, testModel, delay);

            var result = await _coupler.WaitOnAsync<TestModel>(id, TimeSpan.FromSeconds(5));

            var delta = DateTime.Now - start;

            /* Just sanity check ot make sure that we waited for at least how long the process should be decoupled */
            Assert.True(delta.TotalMilliseconds > delay);
            Assert.NotNull(result);
            Assert.True(result.Successful);
            Assert.Equal(testModel.Id, result.Result.Id);
            Assert.Equal(testModel.Name, result.Result.Name);
            Assert.Equal(0, _coupler.ActiveSessions);
        }

        [Fact]
        public async Task Enqueue_Dequeue_Simple_Valid2()
        {
            var id = "C41DE0C9658143E1A4EB00ABFE638CAD";

            var testModel = new TestModel()
            {
                Id = "123456",
                Name = "Some test Data"
            };

            var start = DateTime.Now;

            var delay = 500;

            CompleteIt(id, testModel, delay);

            var result = await _coupler.WaitOnAsync<TestModel>(id, TimeSpan.FromSeconds(5));

            var delta = DateTime.Now - start;

            /* Just sanity check ot make sure that we waited for at least how long the process should be decoupled */
            Assert.True(delta.TotalMilliseconds > delay);
            Assert.NotNull(result);
            Assert.True(result.Successful);
            Assert.Equal(testModel.Id, result.Result.Id);
            Assert.Equal(testModel.Name, result.Result.Name);
            Assert.Equal(0, _coupler.ActiveSessions);
        }

        #region fail condition tests
        [Fact]
        public async Task Enqueue_Dequeue_Generic_Class_CorrelationId_DoesNotExist()
        {
            var id = "C41DE0C9658143E1A4EB00ABFE638CAD";

            var testModel = new TestModel()
            {
                Id = "123456",
                Name = "Some test Data"
            };

            var result = await _coupler.CompleteAsync(id, testModel);

            Assert.NotNull(result);
            Assert.False(result.Successful);
            Assert.Equal($"Correlation id not found: {id}.", result.Errors[0].Message);
            Assert.Equal(0, _coupler.ActiveSessions);
        }

        [Fact]
        public async Task Enqueue_Dequeue_Simple_Timeout_Failed()
        {
            var id = "C41DE0C9658143E1A4EB00ABFE638CAD";

            var testModel = new TestModel()
            {
                Id = "123456",
                Name = "Some test Data"
            };

            var result = await _coupler.WaitOnAsync<TestModel>(id, TimeSpan.FromMilliseconds(500));

            Assert.NotNull(result);
            Assert.False(result.Successful);
            Assert.Equal("Timeout waiting for response.", result.Errors[0].Message);
            Assert.Equal(0, _coupler.ActiveSessions);
        }

        [Fact]
        public async Task Enqueue_Dequeue_WrongObject_Failed()
        {
            var id = "C41DE0C9658143E1A4EB00ABFE638CAD";

            var testModel = new TestModel()
            {
                Id = "123456",
                Name = "Some test Data"
            };
            
            CompleteIt(id, testModel);

            var result = await _coupler.WaitOnAsync<WrongObject>(id, TimeSpan.FromMilliseconds(1500));

            Assert.NotNull(result);
            Assert.False(result.Successful);
            Assert.Equal("Type Mismatch - Expected: WrongObject - Actual: TestModel.", result.Errors[0].Message);
            Assert.Equal(0, _coupler.ActiveSessions);
        }

        [Fact]
        public async Task Enqueue_Dequeue_Null_Failed()
        {
            var id = "C41DE0C9658143E1A4EB00ABFE638CAD";

            var testModel = new TestModel()
            {
                Id = "123456",
                Name = "Some test Data"
            };

            CompleteIt(id, (TestModel)null);

            var result = await _coupler.WaitOnAsync<WrongObject>(id, TimeSpan.FromMilliseconds(1500));

            Assert.NotNull(result);
            Assert.False(result.Successful);
            Assert.Equal("Null Response From Completion Routine.", result.Errors[0].Message);
            Assert.Equal(0, _coupler.ActiveSessions);
        }
        #endregion
    }
}
