namespace LagoVista.Crypto.Modern
{
    public interface IAeadEncryptor
    {
        void Encrypt(byte[] key32, byte[] nonce12, byte[] plaintext, byte[] aad, byte[] ciphertextOut, byte[] tag16Out);
        byte[] Decrypt(byte[] key32, byte[] nonce12, byte[] ciphertext, byte[] tag16, byte[] aad);
    }
}
