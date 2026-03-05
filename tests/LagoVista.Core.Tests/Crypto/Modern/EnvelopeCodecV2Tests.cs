using LagoVista.Crypto.Modern;
using NUnit.Framework;
using System;

namespace LagoVista.Core.Tests.Crypto.Modern
{
    [TestFixture]
    public class EnvelopeCodecV2Tests
    {
        [Test]
        public void BuildParse_RoundTrip_Works()
        {
            var codec = new EnvelopeCodecV2();

            var kv = 1;
            var nonce = new byte[12];
            for (var i = 0; i < nonce.Length; i++) nonce[i] = (byte)i;

            var ciphertext = new byte[] { 10, 11, 12, 13, 14 };
            var tag = new byte[16];
            for (var i = 0; i < tag.Length; i++) tag[i] = (byte)(200 + i);

            var env = codec.Build(kv, nonce, ciphertext, tag);
            Assert.That(env.StartsWith("enc;"), Is.True);
            Assert.That(env.EndsWith(";"), Is.True);

            var parts = codec.Parse(env);
            Assert.That(parts.Kv, Is.EqualTo(kv));
            Assert.That(parts.Nonce, Is.EqualTo(nonce));
            Assert.That(parts.Ciphertext, Is.EqualTo(ciphertext));
            Assert.That(parts.Tag, Is.EqualTo(tag));
        }

        [Test]
        public void Parse_MissingEncPrefix_Throws()
        {
            var codec = new EnvelopeCodecV2();
            Assert.Throws<FormatException>(() => codec.Parse("v=2;"));
        }

        [Test]
        public void Parse_MissingTrailingSemicolon_Throws()
        {
            var codec = new EnvelopeCodecV2();
            Assert.Throws<FormatException>(() => codec.Parse("enc;v=2;alg=a256gcm;kv=1;aad=v1;data=AA"));
        }

        [Test]
        public void Parse_WrongAlg_Throws()
        {
            var codec = new EnvelopeCodecV2();
            var env = "enc;v=2;alg=wrong;kv=1;aad=v1;data=AA;";
            Assert.Throws<FormatException>(() => codec.Parse(env));
        }

        private static EnvelopeCodecV2 NewSut() => new EnvelopeCodecV2();

        private static string BuildValidEnvelope()
        {
            var sut = NewSut();

            var nonce12 = new byte[12];
            var tag16 = new byte[16];
            var ciphertext = Array.Empty<byte>();

            // determinism not required, just valid shape
            return sut.Build(kv: 1, nonce12: nonce12, ciphertext: ciphertext, tag16: tag16);
        }

        [Test]
        public void Parse_WhenMissingKv_ThrowsFormatException()
        {
            var sut = NewSut();
            var valid = BuildValidEnvelope();

            // Remove "kv=1;" entirely
            var broken = valid.Replace("kv=1;", "");

            var ex = Assert.Throws<FormatException>(() => sut.Parse(broken));
            Assert.That(ex.Message, Does.Contain("Missing kv."));
        }

        [Test]
        public void Parse_WhenInvalidKv_ThrowsFormatException()
        {
            var sut = NewSut();
            var valid = BuildValidEnvelope();

            // kv must be > 0, so force kv=0
            var broken = valid.Replace("kv=1;", "kv=0;");

            var ex = Assert.Throws<FormatException>(() => sut.Parse(broken));
            Assert.That(ex.Message, Does.Contain("Invalid kv."));
        }

        [Test]
        public void Parse_WhenMissingData_ThrowsFormatException()
        {
            var sut = NewSut();
            var valid = BuildValidEnvelope();

            // Remove the whole "data=...;" segment
            var dataIdx = valid.IndexOf("data=", StringComparison.Ordinal);
            Assert.That(dataIdx, Is.GreaterThanOrEqualTo(0), "Sanity check: expected data= segment in valid envelope.");

            var dataEnd = valid.IndexOf(';', dataIdx);
            Assert.That(dataEnd, Is.GreaterThan(dataIdx), "Sanity check: expected ';' after data= segment.");

            var broken = valid.Remove(dataIdx, (dataEnd - dataIdx) + 1);

            var ex = Assert.Throws<FormatException>(() => sut.Parse(broken));
            Assert.That(ex.Message, Does.Contain("Missing data."));
        }
    }
}
