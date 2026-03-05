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
    }
}
