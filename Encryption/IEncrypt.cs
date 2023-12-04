namespace GEncryption
{
    internal interface IEncrypt
    {
        public byte[] Encrypt(byte[] buf, int offset, int length);
        public byte[] Decrypt(byte[] buf, int offset, int length);
    }
}
