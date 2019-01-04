#region Header

/*
 * Slovak Technical Services, Inc.
 * Ken Slovak
 * 5/18/17
 */

#endregion

using System;
using System.Text;
using System.IO;
using System.Security.Cryptography;

namespace ShuriOutlookAddIn
{
    internal class Crypto
    {
        #region Constructors

        internal Crypto()
        {
        }

        #endregion

        #region Encryption

         internal string AESEncrypt(string plainText, out string Secret, out string IV)
        {
            string cipherText = "";

            Secret = "";
            IV = "";

            try
            {
                SymmetricAlgorithm aesCrypto = null;


                aesCrypto = SymmetricAlgorithm.Create("AesCryptoServiceProvider");


                aesCrypto.Mode = CipherMode.CBC;
                aesCrypto.Padding = PaddingMode.PKCS7;
                aesCrypto.BlockSize = 128;
                aesCrypto.KeySize = 256;
                aesCrypto.FeedbackSize = 128;

                // Convert our plaintext into a byte array.
                // Let us assume that plaintext contains UTF8-encoded characters.
                byte[] plainTextBytes = Encoding.UTF8.GetBytes(plainText);

                // Generate encryptor from the existing key bytes and initialization 
                // vector. Key size will be defined based on the number of the key 
                // bytes.
                ICryptoTransform encryptor = aesCrypto.CreateEncryptor();

                // Define memory stream which will be used to hold encrypted data.
                MemoryStream memoryStream = new MemoryStream();

                // Define cryptographic stream (always use Write mode for encryption).
                CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write);
                // Start encrypting.
                cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);

                // Finish encrypting.
                cryptoStream.FlushFinalBlock();

                // Convert our encrypted data from a memory stream into a byte array.
                byte[] cipherTextBytes = memoryStream.ToArray();

                // Close both streams.
                memoryStream.Close();
                cryptoStream.Close();

                // Convert encrypted data into a base64-encoded string.
                cipherText = Convert.ToBase64String(cipherTextBytes);

                // get key and IV
                byte[] secret_key = aesCrypto.Key;
                byte[] iv = aesCrypto.IV;

                Secret = Convert.ToBase64String(secret_key);
                IV = Convert.ToBase64String(iv);

            }
            catch 
            {
                cipherText = "";
                Secret = "";
                IV = "";
            }

            // Return encrypted string.
            return cipherText;
        }

        internal string AESDecrypt(string cipherText, string Secret, string IV)
        {
            string plainText = "";

            try
            {
                byte[] secret_key = Convert.FromBase64String(Secret);
                byte[] iv = Convert.FromBase64String(IV); // get from base64 string

                // Convert our ciphertext into a byte array from from base64 string
                byte[] cipherTextBytes = Convert.FromBase64String(cipherText);

                // alternative object creation method
                SymmetricAlgorithm aesCrypto = SymmetricAlgorithm.Create("AesCryptoServiceProvider");

                // paramters
                aesCrypto.Mode = CipherMode.CBC;
                aesCrypto.Padding = PaddingMode.PKCS7;
                aesCrypto.BlockSize = 128;
                aesCrypto.KeySize = 256;
                aesCrypto.FeedbackSize = 128;

                // Generate decryptor from the existing key bytes and initialization 
                // vector. Key size will be defined based on the number of the key 
                // bytes.
                ICryptoTransform decryptor = aesCrypto.CreateDecryptor(secret_key, iv);

                // Define memory stream which will be used to hold encrypted data.
                MemoryStream memoryStream = new MemoryStream(cipherTextBytes);

                // Define cryptographic stream (always use Read mode for encryption).
                CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read);

                // Since at this point we don't know what the size of decrypted data
                // will be, allocate the buffer long enough to hold ciphertext;
                // plaintext is never longer than ciphertext.
                byte[] plainTextBytes = new byte[cipherTextBytes.Length];

                // Start decrypting.
                int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);

                // Close both streams.
                memoryStream.Close();
                cryptoStream.Close();

                // Convert decrypted data into a string. 
                // Let us assume that the original plaintext string was UTF8-encoded.
                plainText = Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
            }
            catch
            {
                plainText = "";
            }

            // Return decrypted string.   
            return plainText;
        }

        #endregion  
    }
}
