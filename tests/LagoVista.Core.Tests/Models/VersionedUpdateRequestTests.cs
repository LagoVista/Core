using LagoVista.Core.Models;
using NUnit.Framework;

namespace LagoVista.Core.Tests.Models
{
    [TestFixture]
    public class VersionedUpdateRequestTests
    {
        private sealed class TestRecord
        {
            public string Id { get; set; }
        }

        [Test]
        public void VersionedUpdateRequest_Should_Preserve_Record_And_ExpectedVersion()
        {
            var record = new TestRecord { Id = "record-1" };
            var request = new VersionedUpdateRequest<TestRecord>
            {
                Record = record,
                ExpectedVersion = "version-42"
            };

            Assert.That(request.Record, Is.SameAs(record));
            Assert.That(request.ExpectedVersion, Is.EqualTo("version-42"));
        }
    }
}
