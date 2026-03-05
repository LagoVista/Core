namespace LagoVista.Crypto.Modern
{
    public interface IEnvelopeCodec
    {
        string Build(int kv, byte[] nonce12, byte[] ciphertext, byte[] tag16);
        EnvelopeParts Parse(string envelope);
    }

    public sealed class EnvelopeParts
    {
        public int Kv { get; set; }
        public byte[] Nonce { get; set; }
        public byte[] Ciphertext { get; set; }
        public byte[] Tag { get; set; }
    }
}
