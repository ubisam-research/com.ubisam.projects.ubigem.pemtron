using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Remoting.Messaging;
using System.Security.Cryptography;
using System.Text;

namespace UbiSam.Net.KeyLock.Utilities
{

    public class Cryptography
    {
        private static byte[] GetIV01()
        {
            string v = "IV_KEY_01";
            byte[] IV = new byte[16];
            int i, n;
            for (i = 0, n = v.Length; i < n; i++)
            {
                IV[i] = (byte)v[i];
            }
            for (n = 16; i < n; i++)
            {
                IV[i] = 0;
            }
            return IV;
        }
        private static byte[] GetIV02()
        {
            string v = "IV_KEY_02";
            byte[] IV = new byte[16];
            int i, n;
            for (i = 0, n = v.Length; i < n; i++)
            {
                IV[i] = (byte)v[i];
            }
            for (n = 16; i < n; i++)
            {
                IV[i] = 0;
            }
            return IV;
        }
        private static byte[] GetIV1()
        {
            string v = "UBISAM_KEYLOCK";
            byte[] IV = new byte[16];
            int i, n;
            for (i = 0, n = v.Length; i < n; i++)
            {
                IV[i] = (byte)v[i];
            }
            for (n = 16; i < n; i++)
            {
                IV[i] = 0;
            }
            return IV;
        }

        private static byte[] iv2
        {
            get
            {
                // length must be 16
                string v = "UBISAM_KEYLOCKV2";
                byte[] IV = new byte[16];
                int i, n;
                for (i = 0, n = v.Length; i < n; i++)
                {
                    IV[i] = (byte)v[i];
                }
                return IV;
            }
        }

        public static string Encrypt1(string plainText)
        {
            // Using Rijndael algorithm for AES-256 encryption
            RijndaelManaged aes = new RijndaelManaged();
            // KeySize: 256
            aes.KeySize = 256;
            // BlockSize: Initial vector size: 128
            aes.BlockSize = 128;
            // CipherMode CBC
            aes.Mode = CipherMode.CBC;
            // key padding: pkcs7
            aes.Padding = PaddingMode.PKCS7;
            string newKey = "IV_KEY_01";
            while (newKey.Length < 32)
            {
                newKey = string.Format("{0}{1}", newKey, " ");
            }

            aes.Key = Encoding.UTF8.GetBytes(newKey);

            aes.IV = GetIV01();
            var encrypt = aes.CreateEncryptor(aes.Key, aes.IV);
            byte[] xBuff = null;
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, encrypt, CryptoStreamMode.Write))
                {
                    byte[] xXml = Encoding.UTF8.GetBytes(plainText);
                    cs.Write(xXml, 0, xXml.Length);
                }

                xBuff = ms.ToArray();
            }

            string Output = Convert.ToBase64String(xBuff);
            xBuff = null;

            encrypt.Dispose();
            GC.SuppressFinalize(encrypt);

            aes.Dispose();
            GC.SuppressFinalize(aes);

            return Output;
        }

        public static string Decrypt1(string cipherText)
        {
            string Output;

            // Using Rijndael algorithm for AES-256 dencryption
            RijndaelManaged aes = new RijndaelManaged();
            // KeySize: 256
            aes.KeySize = 256;
            // BlockSize: Initial vector size: 128
            aes.BlockSize = 128;
            // CipherMode CBC
            aes.Mode = CipherMode.CBC;
            // key padding: pkcs7
            aes.Padding = PaddingMode.PKCS7;

            string newKey = "IV_KEY_01";
            while (newKey.Length < 32)
            {
                newKey = string.Format("{0}{1}", newKey, " ");
            }

            aes.Key = Encoding.UTF8.GetBytes(newKey);
            aes.IV = GetIV01();

            var decrypt = aes.CreateDecryptor();
            byte[] xBuff = null;

            try
            {
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, decrypt, CryptoStreamMode.Write))
                    {
                        byte[] xXml = Convert.FromBase64String(cipherText);
                        cs.Write(xXml, 0, xXml.Length);
                    }

                    xBuff = ms.ToArray();
                }

                Output = Encoding.UTF8.GetString(xBuff);
                xBuff = null;
            }
            catch
            {
                Output = string.Empty;
            }

            decrypt.Dispose();
            GC.SuppressFinalize(decrypt);

            aes.Dispose();
            GC.SuppressFinalize(aes);

            return Output;
        }

        public static string Encrypt2(string plainText)
        {
            // Using Rijndael algorithm for AES-256 encryption
            RijndaelManaged aes = new RijndaelManaged();
            // KeySize: 256
            aes.KeySize = 256;
            // BlockSize: Initial vector size: 128
            aes.BlockSize = 128;
            // CipherMode CBC
            aes.Mode = CipherMode.CBC;
            // key padding: pkcs7
            aes.Padding = PaddingMode.PKCS7;
            string newKey = "IV_KEY_02";

            string Output;

            while (newKey.Length < 32)
            {
                newKey = string.Format("{0}{1}", newKey, " ");
            }

            aes.Key = Encoding.UTF8.GetBytes(newKey);

            aes.IV = GetIV02();
            var encrypt = aes.CreateEncryptor(aes.Key, aes.IV);
            byte[] xBuff = null;

            if (plainText != null)
            {
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, encrypt, CryptoStreamMode.Write))
                    {
                        byte[] xXml = Encoding.UTF8.GetBytes(plainText);
                        cs.Write(xXml, 0, xXml.Length);
                    }

                    xBuff = ms.ToArray();
                }

                Output = Convert.ToBase64String(xBuff);
                xBuff = null;
            }
            else
            {
                Output = string.Empty;
            }

            encrypt.Dispose();
            GC.SuppressFinalize(encrypt);

            aes.Dispose();
            GC.SuppressFinalize(aes);

            return Output;
        }

        public static string Decrypt2(string cipherText)
        {
            string Output;

            // Using Rijndael algorithm for AES-256 dencryption
            RijndaelManaged aes = new RijndaelManaged();
            // KeySize: 256
            aes.KeySize = 256;
            // BlockSize: Initial vector size: 128
            aes.BlockSize = 128;
            // CipherMode CBC
            aes.Mode = CipherMode.CBC;
            // key padding: pkcs7
            aes.Padding = PaddingMode.PKCS7;

            string newKey = "IV_KEY_02";
            while (newKey.Length < 32)
            {
                newKey = string.Format("{0}{1}", newKey, " ");
            }

            aes.Key = Encoding.UTF8.GetBytes(newKey);
            aes.IV = GetIV02();

            var decrypt = aes.CreateDecryptor();
            byte[] xBuff = null;

            try
            {
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, decrypt, CryptoStreamMode.Write))
                    {
                        byte[] xXml = Convert.FromBase64String(cipherText);
                        cs.Write(xXml, 0, xXml.Length);
                    }

                    xBuff = ms.ToArray();
                }

                Output = Encoding.UTF8.GetString(xBuff);
                xBuff = null;
            }
            catch
            {
                Output = string.Empty;
            }

            decrypt.Dispose();
            GC.SuppressFinalize(decrypt);

            aes.Dispose();
            GC.SuppressFinalize(aes);

            return Output;
        }

        public static string Encrypt8(string plainText, string key)
        {
            // Using Rijndael algorithm for AES-256 encryption
            RijndaelManaged aes = new RijndaelManaged();
            // KeySize: 256
            aes.KeySize = 256;
            // BlockSize: Initial vector size: 128
            aes.BlockSize = 128;
            // CipherMode CBC
            aes.Mode = CipherMode.CBC;
            // key padding: pkcs7
            aes.Padding = PaddingMode.PKCS7;
            string newKey = key;
            while( newKey.Length < 32 )
            {
                newKey = string.Format("{0}{1}", newKey, " ");
            }

            aes.Key = Encoding.UTF8.GetBytes(newKey);

            aes.IV = GetIV1();
            var encrypt = aes.CreateEncryptor(aes.Key, aes.IV);
            byte[] xBuff = null;
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, encrypt, CryptoStreamMode.Write))
                {
                    byte[] xXml = Encoding.UTF8.GetBytes(plainText);
                    cs.Write(xXml, 0, xXml.Length);
                }

                xBuff = ms.ToArray();
            }

            string Output = Convert.ToBase64String(xBuff);
            xBuff = null;

            encrypt.Dispose();
            GC.SuppressFinalize(encrypt);

            aes.Dispose();
            GC.SuppressFinalize(aes);

            return Output;
        }

        public static string Decrypt8(string cipherText, string key)
        {
            string Output;

            // Using Rijndael algorithm for AES-256 dencryption
            RijndaelManaged aes = new RijndaelManaged();
            // KeySize: 256
            aes.KeySize = 256;
            // BlockSize: Initial vector size: 128
            aes.BlockSize = 128;
            // CipherMode CBC
            aes.Mode = CipherMode.CBC;
            // key padding: pkcs7
            aes.Padding = PaddingMode.PKCS7;

            string newKey = key;
            while (newKey.Length < 32)
            {
                newKey = string.Format("{0}{1}", newKey, " ");
            }

            aes.Key = Encoding.UTF8.GetBytes(newKey);
            aes.IV = GetIV1();

            var decrypt = aes.CreateDecryptor();
            byte[] xBuff = null;

            try
            {
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, decrypt, CryptoStreamMode.Write))
                    {
                        byte[] xXml = Convert.FromBase64String(cipherText);
                        cs.Write(xXml, 0, xXml.Length);
                    }

                    xBuff = ms.ToArray();
                }

                Output = Encoding.UTF8.GetString(xBuff);
                xBuff = null;
            }
            catch
            {
                Output = string.Empty;
            }

            decrypt.Dispose();
            GC.SuppressFinalize(decrypt);

            aes.Dispose();
            GC.SuppressFinalize(aes);

            return Output;
        }

        public static string Encrypt9(string message, string salt)
        {
            // Using SHA512 for hashing
            SHA512 sha = SHA512Managed.Create();
            // Combine message and salt
            string salted = string.Format("{0}{1}", message, salt);

            byte[] hash = Encoding.UTF8.GetBytes(salted);
            // Hashing for 50000 times
            for( int i = 0; i < 50000; i++ )
            {
                hash = sha.ComputeHash(hash);
            }

            sha.Dispose();
            GC.SuppressFinalize(sha);
            StringBuilder sbBuff = new StringBuilder();

            foreach(byte x in hash)
            {
                sbBuff.Append(x.ToString("X2"));
            }
            return sbBuff.ToString();
        }

        public static string Encrypt3(string plainText, string key)
        {
            // Using Rijndael algorithm for AES-256 encryption
            RijndaelManaged aes = new RijndaelManaged();
            // KeySize: 256
            aes.KeySize = 256;
            // BlockSize: Initial vector size: 128
            aes.BlockSize = 128;
            // CipherMode CBC
            aes.Mode = CipherMode.CBC;
            // key padding: pkcs7
            aes.Padding = PaddingMode.PKCS7;

            string newKey = key;
            while (newKey.Length < 32)
            {
                newKey = string.Format("{0}{1}", newKey, " ");
            }

            aes.Key = Encoding.UTF8.GetBytes(newKey);

            aes.IV = iv2;
            var encrypt = aes.CreateEncryptor(aes.Key, aes.IV);
            byte[] xBuff = null;
            using (var ms = new MemoryStream())
            {
                using (var cs = new CryptoStream(ms, encrypt, CryptoStreamMode.Write))
                {
                    byte[] xXml = Encoding.UTF8.GetBytes(plainText);
                    cs.Write(xXml, 0, xXml.Length);
                }

                xBuff = ms.ToArray();
            }

            string Output = Convert.ToBase64String(xBuff);

            encrypt.Dispose();
            GC.SuppressFinalize(encrypt);

            aes.Dispose();
            GC.SuppressFinalize(aes);

            xBuff = null;
            return Output;
        }

        public static string Decrypt3(string cipherText, string key)
        {
            string Output;

            // Using Rijndael algorithm for AES-256 dencryption
            RijndaelManaged aes = new RijndaelManaged();
            // KeySize: 256
            aes.KeySize = 256;
            // BlockSize: Initial vector size: 128
            aes.BlockSize = 128;
            // CipherMode CBC
            aes.Mode = CipherMode.CBC;
            // key padding: pkcs7
            aes.Padding = PaddingMode.PKCS7;

            string newKey = key;
            while (newKey.Length < 32)
            {
                newKey = string.Format("{0}{1}", newKey, " ");
            }

            aes.Key = Encoding.UTF8.GetBytes(newKey);
            aes.IV = iv2;

            var decrypt = aes.CreateDecryptor();
            byte[] xBuff = null;

            try
            {
                using (var ms = new MemoryStream())
                {
                    using (var cs = new CryptoStream(ms, decrypt, CryptoStreamMode.Write))
                    {
                        byte[] xXml = Convert.FromBase64String(cipherText);
                        cs.Write(xXml, 0, xXml.Length);
                    }

                    xBuff = ms.ToArray();
                }

                Output = Encoding.UTF8.GetString(xBuff);
                xBuff = null;
            }
            catch
            {
                Output = string.Empty;
            }

            decrypt.Dispose();
            GC.SuppressFinalize(decrypt);

            aes.Dispose();
            GC.SuppressFinalize(aes);

            return Output;
        }

        public static string Encrypt4(string message, string salt)
        {
            // Using SHA512 for hashing
            SHA512 sha = SHA512Managed.Create();
            // Combine message and salt
            string salted = string.Format("{0}{1}", message, salt);

            byte[] hash = Encoding.UTF8.GetBytes(salted);
            // Hashing for 50000 times
            for (int i = 0; i < 49999; i++)
            {
                hash = sha.ComputeHash(hash);
            }

            StringBuilder sbBuff = new StringBuilder();

            foreach (byte x in hash)
            {
                sbBuff.Append(x.ToString("X2"));
            }

            sha.Dispose();
            GC.SuppressFinalize(sha);

            return sbBuff.ToString();
        }

        public static string[] MakeKeyLockV1(string plainText, string volumeName, string secretKey)
        {
            string salt = Encrypt8(plainText, volumeName);
            string sha = Encrypt9(secretKey, salt);
            return new string[] { salt, sha };
        }
        public static string[] MakeKeyLockV2(string plainText, string volumeName, string secretKey)
        {
            string version = Encrypt3("V2|USB", volumeName);
            string salt = Encrypt3(plainText, version);
            string sha = Encrypt4(secretKey, version);
            return new string[] { version, salt, sha };
        }

        public static string[] MakeSystemKeyV2(string plainText, params string[] secretKey)
        {
            List<string> result;

            result = new List<string>();

            string version = Encrypt3("V2|SYSTEM", "UUKL");
            result.Add(version);

            string salt = Encrypt3(plainText, version);
            result.Add(salt);

            for (int i = 0; i < secretKey.Length; i++)
            {
                result.Add(Encrypt4(secretKey[i], version));
            }
            return result.ToArray();
        }
    }
}
