namespace Plisky.Plumbing {

    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// Implements a cryptographic helper for 24 bit keys.  This is probably a naieve implementation but it iwll server as a starter.
    /// </summary>
    public static class CryptoHelper {
        private const int KEYLENGTH = 24;

        public static string DecryptValue(byte[] key, string data) {
            byte[] clearData = Convert.FromBase64String(data);
            //clearData = System.Text.Encoding.UTF8.GetBytes(data);

            string result;
            try {
                using (TripleDESCryptoServiceProvider cp = new TripleDESCryptoServiceProvider()) {
                    cp.Key = key;
                    cp.IV = GetIV();
                    using (var ms = new MemoryStream(clearData, false)) {
                        using (CryptoStream strm = new CryptoStream(ms, cp.CreateDecryptor(), CryptoStreamMode.Read)) {
                            using (StreamReader sr = new StreamReader(strm)) {
                                result = sr.ReadToEnd();
                            }
                        }
                    }
                }
            } catch (ArgumentNullException) {
                // Not sure why this occurs, think it might come from the reader if the password is invalid
                result = null;
            } catch (CryptographicException) {
                result = null;
            }
            return result;
        }

        public static string EncryptValue(byte[] key, string dataToEncrypt) {
            byte[] clearData = Encoding.UTF8.GetBytes(dataToEncrypt);

            string result;

            using (TripleDESCryptoServiceProvider cp = new TripleDESCryptoServiceProvider()) {
                cp.Key = key;
                cp.IV = GetIV();

                using (var ms = new MemoryStream()) {
                    using (CryptoStream strm = new CryptoStream(ms, cp.CreateEncryptor(), CryptoStreamMode.Write)) {
                        strm.Write(clearData, 0, clearData.Length);
                        strm.FlushFinalBlock();
                    }

                    byte[] encrpytedData = ms.ToArray();
                    result = Convert.ToBase64String(encrpytedData);
                }
            }

            return result;
        }

        /// <summary>
        /// This will use the key identity as a seed to generate a key which is used to encrypt the data, this can be a way of obsfuscating the actual
        /// key by using an int value called something else to pretend to be the key. It is not very secure at all but it is harder to work out
        /// whats actually going on.
        /// </summary>
        /// <param name="keyIdentity">The identity to seed the key generator with</param>
        /// <returns>A Key which can be used for encrypting data</returns>
        public static byte[] GenerateKeyFromIdentity(int keyIdentity) {
            Random r = new Random(keyIdentity);
            byte[] key = new byte[KEYLENGTH];
            r.NextBytes(key);
            return key;
        }

        public static byte[] GetIV() {
            byte[] iv = { 1, 4, 7, 3, 4, 7, 8, 0 };
            return iv;
        }
    }
}