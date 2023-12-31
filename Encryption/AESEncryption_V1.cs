﻿using System.Security.Cryptography;

namespace GHelpers
{

    public class EncryptionSection
    {
        public string? Key { get; set; }
        public string? Iv { get; set; }
    }

    class AESEncryption_V1 : IEncrypt
    {
        readonly byte[] _key = { 51, 53, 212, 118, 217, 153, 66, 236, 26, 221, 196, 54, 255, 126, 144, 159, 64, 168, 156, 222, 221, 169, 63, 172, 160, 243, 128, 97, 16, 253, 156, 86 };
        readonly byte[] _iv = { 242, 245, 156, 211, 170, 71, 78, 226, 75, 89, 105, 154, 133, 183, 251, 33 };

        readonly Aes _aesAlg;
        readonly ICryptoTransform _encryptor;
        readonly ICryptoTransform _decryptor;

        public AESEncryption_V1(EncryptionSection? s)
        {
            if (s != null)
            {
                var akey = s.Key?.Split(',');
                var aiv = s.Iv?.Split(',');
                akey.ThrowIfArrayIsShort(32, "Key must be 32 numbers string separated by comma");
                aiv.ThrowIfArrayIsShort(16, "Iv must be 16 numbers string separated by comma");

                for (int i = 0; i < 32; i++)
                    _key[i] = (byte)int.Parse(akey[i].Trim());

                for (int i = 0; i < 16; i++)
                    _iv[i] = (byte)int.Parse(aiv[i].Trim());
            }
            _aesAlg = Aes.Create("AesManaged");
            _aesAlg.KeySize = 256;
            _aesAlg.BlockSize = 128;

            _encryptor = _aesAlg.CreateEncryptor(_key, _iv);
            _decryptor = _aesAlg.CreateDecryptor(_key, _iv);
        }

        public byte[] Encrypt(byte[] buf, int offset, int count)
        {
            lock (_encryptor)
            {
                return _encryptor.TransformFinalBlock(buf, 0, buf.Length);
            }
        }

        public byte[] Decrypt(byte[] buf, int offset, int count)
        {
            lock (_decryptor)
            {
                return _decryptor.TransformFinalBlock(buf, 0, buf.Length);
            }
        }
    }
}
