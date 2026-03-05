using LagoVista.Core.Models;
using LagoVista.Crypto.Modern;
using NUnit.Framework;
using System;
using System.Text;

namespace LagoVista.Core.Tests.Crypto.Modern
{
    [TestFixture]
    public class AadBuilderV1Tests
    {
        [Test]
        public void BuildAadV1_HasExpectedPrefixAndLengths()
        {
            var builder = new AadBuilderV1();

            var orgId = (GuidString36)("11111111-1111-1111-1111-111111111111");
            var recId = (GuidString36)("22222222-2222-2222-2222-222222222222");
            var field = "encryptedbalance";
            var keyId = "account-0123456789abcdef0123456789abcdef:v2";

            var aad = builder.BuildAadV1(orgId, recId, field, keyId);

            // prefix
            Assert.That(Encoding.ASCII.GetString(aad, 0, 4), Is.EqualTo("aad1"));

            // total length sanity: 4 + 16 + 16 + 2 + field + 2 + keyId
            var expected = 4 + 16 + 16 + 2 + Encoding.UTF8.GetByteCount(field) + 2 + Encoding.UTF8.GetByteCount(keyId);
            Assert.That(aad.Length, Is.EqualTo(expected));
        }

        [Test]
        public void BuildAadV1_NullField_ThrowsArgumentNullException()
        {
            var builder = new AadBuilderV1();
            var orgId = GuidString36.Factory();
            var recId = GuidString36.Factory();

            Assert.Throws<ArgumentNullException>(() => builder.BuildAadV1(orgId, recId, null, "kid"));
        }
    }
}
