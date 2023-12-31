﻿using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Diagnostics.CodeAnalysis;

namespace GHelpers
{
    public static class EncryptionHelper
    {
        [AllowNull]
        static IEncrypt _encryptorA;

        public static string Encrypt(this string s)
        {
            if (string.IsNullOrEmpty(s))
                return s;
            var buf = System.Text.Encoding.ASCII.GetBytes(s);
            return 'A' + Convert.ToBase64String(_encryptorA.Encrypt(buf, 0, buf.Length));
        }

        public static string Decrypt(this string s)
        {
            if (string.IsNullOrEmpty(s))
                return s;

            var buf = Convert.FromBase64String(s[1..]);
            return s[0] switch
            {
                'A' => System.Text.Encoding.ASCII.GetString(_encryptorA.Decrypt(buf, 0, buf.Length)),
                _ => throw new Exception("Uknown encryption"),
            };
        }

        public static IServiceCollection UseEncryption(this IServiceCollection serviceCollection, IConfiguration config, string sectionName = "Encryption")
        {
            var sec = config.GetSection(sectionName).Get<EncryptionSection>();
            var encryptor = new AESEncryption_V1(sec);
            _encryptorA = encryptor;
            return serviceCollection;
        }
    }
}
