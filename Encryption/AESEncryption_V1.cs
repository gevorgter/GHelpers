using System;
using System.Diagnostics.CodeAnalysis;
using System.Security.Cryptography;

namespace GEncryption
{

    public class EncryptionSection
    {
        [AllowNull]
        public string Key { get; set; }
        [AllowNull]
        public string Iv { get; set; }
    }

    class AESEncryption_V1 : IEncrypt
    {
        //The key size must be 256 bites.
        public byte[] _key = { 51, 53, 212, 118, 217, 153, 66, 236, 26, 221, 196, 54, 255, 126, 144, 159, 64, 168, 156, 222, 221, 169, 63, 172, 160, 243, 128, 97, 16, 253, 156, 86 };
        public byte[] _iv = { 242, 245, 156, 211, 170, 71, 78, 226, 75, 89, 105, 154, 133, 183, 251, 33 };

        [AllowNull]
        Aes _aesAlg = null;
        [AllowNull]
        ICryptoTransform _encryptor;
        [AllowNull]
        ICryptoTransform _decryptor;

        public void Init(EncryptionSection? s)
        {
            if (s != null)
            {
                var akey = s.Key.Split(',');
                var aiv = s.Iv.Split(',');

                if (akey.Length != 32)
                    throw new ArgumentException("Key must be 32 numbers string separated by comma");
                if (aiv.Length != 16)
                    throw new ArgumentException("Iv must be 16 numbers string separated by comma");

                for (int i = 0; i < akey.Length; i++)
                    _key[i] = (byte)int.Parse(akey[i].Trim());

                for (int i = 0; i < aiv.Length; i++)
                    _iv[i] = (byte)int.Parse(aiv[i].Trim());
            }
            _aesAlg = Aes.Create("AesManaged");
            _aesAlg.KeySize = 256;
            _aesAlg.BlockSize = 128;

            _encryptor = _aesAlg.CreateEncryptor(_key, _iv);
            _decryptor = _aesAlg.CreateDecryptor(_key, _iv);
        }

        static byte[] PerformCryptography(ICryptoTransform cryptoTransform, byte[] buffer, int offset, int count)
        {
            using var memoryStream = GHelpers.GHelper._recyclableMemoryStreamManager.GetStream();
            using var cryptoStream = new CryptoStream(memoryStream, cryptoTransform, CryptoStreamMode.Write);
            cryptoStream.Write(buffer, offset, count);
            cryptoStream.FlushFinalBlock();
            return memoryStream.ToArray();
        }

        public byte[] Encrypt(byte[] buf, int offset, int count)
        {
            lock (_encryptor)
            {
                return PerformCryptography(_encryptor, buf, offset, count);
            }
        }

        public byte[] Decrypt(byte[] buf, int offset, int count)
        {
            lock (_decryptor)
            {
                return PerformCryptography(_decryptor, buf, offset, count);
            }
        }
    }
}
