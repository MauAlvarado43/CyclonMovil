using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace Cyclon.Utils {
    class Cipher
    {

        private static string _key = "";
        private static string _iv = "";

        public static string encrypt(string plainText) {

            byte[] encrypted;

            using (Aes aes = Aes.Create()) {

                aes.Key = Convert.FromBase64String(_key);
                aes.IV = Convert.FromBase64String(_iv);

                using (MemoryStream memoryStream = new MemoryStream()) {

                    using (ICryptoTransform encryptor = aes.CreateEncryptor())
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))

                    using (StreamWriter streamWriter = new StreamWriter(cryptoStream)) {
                        streamWriter.Write(plainText);
                    }

                    encrypted = memoryStream.ToArray();

                }
            }

            return Convert.ToBase64String(encrypted);

        }

        public static string decrypt(string encryptedValue) {

            byte[] cipherText = Convert.FromBase64String(encryptedValue);
            string plaintext = null;

            using (Aes aes = Aes.Create()) {

                aes.Key = Convert.FromBase64String(_key);
                aes.IV = Convert.FromBase64String(_iv);

                using (MemoryStream memoryStream = new MemoryStream(cipherText))
                using (ICryptoTransform decryptor = aes.CreateDecryptor())
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                using (StreamReader streamReader = new StreamReader(cryptoStream))
                
                plaintext = streamReader.ReadToEnd();

            }

            return plaintext;

        }


    }

}
