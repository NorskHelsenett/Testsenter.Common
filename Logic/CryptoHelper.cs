using Newtonsoft.Json;
using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using Shared.Common.Storage;

// ReSharper disable PossibleNullReferenceException
// ReSharper disable UnusedMember.Local

namespace Shared.Common.Logic
{
    public static class CryptoHelper
    {
        public static string CalculateMD5Hash(string input)
        {
            var md5 = System.Security.Cryptography.MD5.Create();

            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < hash.Length; i++)
                sb.Append(hash[i].ToString("X2"));

            return sb.ToString();
        }


        public static X509Certificate2 GetCertificateFromBlobStorage(string thumb, string privatekeypassword, IBlobStorageDb blobStorage)
        {
            var certBlob = blobStorage.GetBytes(thumb);
            return new X509Certificate2(certBlob, privatekeypassword, X509KeyStorageFlags.Exportable);
        }

        public static byte[] GetEncryptedContent(byte[] b, X509Certificate2 certificate)
        {
            RSACryptoServiceProvider rsa = certificate.PublicKey.Key as RSACryptoServiceProvider;
            return rsa.Encrypt(b, true);
        }

        public static byte[] GetDecryptedContent(byte[] bytes, X509Certificate2 certificate)
        {
            if (!certificate.HasPrivateKey)
                throw new Exception("Does not have the private key. Cannot decrypt");

            RSACryptoServiceProvider rsa = certificate.PrivateKey as RSACryptoServiceProvider;
            return rsa.Decrypt(bytes, true);
        }

        public static byte[] GetBytes(this string str)
        {
            byte[] bytes = new byte[str.Length * sizeof(char)];
            Buffer.BlockCopy(str.ToCharArray(), 0, bytes, 0, bytes.Length);
            return bytes;
        }

        public static string GetString(this byte[] bytes)
        {
            char[] chars = new char[bytes.Length / sizeof(char)];
            Buffer.BlockCopy(bytes, 0, chars, 0, bytes.Length);
            return new string(chars);
        }

        private static byte[] AsByteArray<T>(this T obj)
        {
            if (obj == null)
                return null;

            BinaryFormatter bf = new BinaryFormatter();
            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, obj);
                return ms.ToArray();
            }
        }

        private static T ToObject<T>(this byte[] arrBytes)
        {
            using (MemoryStream memStream = new MemoryStream())
            {
                BinaryFormatter binForm = new BinaryFormatter();
                memStream.Write(arrBytes, 0, arrBytes.Length);
                memStream.Seek(0, SeekOrigin.Begin);
                var obj = binForm.Deserialize(memStream);

                return (T) obj;
            }
        }

        public static class Encryptor
        {
            private static SymmetricAlgorithm _cryptoService = new TripleDESCryptoServiceProvider();
            public static string Encrypt(string text, byte[] key, byte[] vector)
            {
                var asBase64 = Convert.ToBase64String(Encoding.Default.GetBytes(text));
                return TransformBase64(asBase64, _cryptoService.CreateEncryptor(key, vector));
            }

            public static string Decrypt(string encryptedBase64, byte[] key, byte[] vector)
            {
                var base64 = TransformBase64(encryptedBase64, _cryptoService.CreateDecryptor(key, vector));
                var clearText = Encoding.Default.GetString(Convert.FromBase64String(base64));

                return clearText;
            }

            private static string TransformBase64(string text, ICryptoTransform cryptoTransform)
            {
                MemoryStream stream = new MemoryStream();
                CryptoStream cryptoStream = new CryptoStream(stream, cryptoTransform, CryptoStreamMode.Write);

                byte[] input = Convert.FromBase64String(text);

                cryptoStream.Write(input, 0, input.Length);
                cryptoStream.FlushFinalBlock();

                return Convert.ToBase64String(stream.ToArray());
            }
        }

        public static class SimpleStringCrypt
        {
            private static byte[] key = new byte[8] { 8, 1, 3, 4, 5, 6, 2, 8 };

            public static string Crypt(string text)
            {
                SymmetricAlgorithm algorithm = DES.Create();
                ICryptoTransform transform = algorithm.CreateEncryptor(key, key);
                byte[] inputbuffer = Encoding.Unicode.GetBytes(text);
                byte[] outputBuffer = transform.TransformFinalBlock(inputbuffer, 0, inputbuffer.Length);
                return Convert.ToBase64String(outputBuffer);
            }

            public static string Decrypt(string text)
            {
                SymmetricAlgorithm algorithm = DES.Create();
                ICryptoTransform transform = algorithm.CreateDecryptor(key, key);
                byte[] inputbuffer = Convert.FromBase64String(text);
                byte[] outputBuffer = transform.TransformFinalBlock(inputbuffer, 0, inputbuffer.Length);
                return Encoding.Unicode.GetString(outputBuffer);
            }
        }
    }
}
