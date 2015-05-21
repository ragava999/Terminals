///////////////////////////////////////////////////////////////////////////////
// SAMPLE: Symmetric key encryption and decryption using Rijndael algorithm.
// 
// To run this sample, create a new Visual C# project using the Console
// Application template and replace the contents of the Class1.cs file with
// the code below.
//
// THIS CODE AND INFORMATION IS PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED 
// WARRANTIES OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// 
// Copyright (C) 2002 Obviex(TM). All rights reserved.
// 

using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using Rug.Cmd;

namespace Rpx.Packing.Helpers
{
    internal class EncryptHelper
    {
        private const string HashAlgorithm = "SHA1";
        internal const string InitVector = "@1B2c3D4e5F6g7H8";
        internal const string SaltValue = "s@1tValue";
        internal const int KeySize = 256;

        public static void EncryptFile(string path, string password, uint strength)
        {
            EncryptFile(path, password, SaltValue, strength);
        }

        public static void EncryptFile(string path, string password, string saltString, uint strength)
        {
            if (!File.Exists(path))
                throw new Exception(string.Format(Rpx.Strings.Encrypt_ErrorPathDoesNotExist, path));

            CmdHelper.WriteInfoToConsole(Rpx.Strings.Hide_Protection, Rpx.Strings.Hide_Encrypted,  RC.Theme[ConsoleThemeColor.TextGood]);
            CmdHelper.WriteInfoToConsole("- " + Rpx.Strings.Encypt_Password, password,  RC.Theme[ConsoleThemeColor.SubText], true);
            CmdHelper.WriteInfoToConsole("- " + Rpx.Strings.Encypt_Salt, saltString, RC.Theme[ ConsoleThemeColor.SubText], true);
            CmdHelper.WriteInfoToConsole(ConsoleVerbosity.Verbose, "- " + Rpx.Strings.Encypt_Strength, strength.ToString(), RC.Theme[ConsoleThemeColor.SubText]);

            byte[] bytes = File.ReadAllBytes(path);

            CmdHelper.WriteInfoToConsole(ConsoleVerbosity.Verbose, "- " + Rpx.Strings.Encypt_OriginalSize, CmdHelper.GetMemStringFromBytes(bytes.Length, true), RC.Theme[ConsoleThemeColor.SubText]);
            
            byte[] encryptedBytes = Encrypt(bytes, password, saltString, HashAlgorithm, (int)strength, InitVector, KeySize);

            CmdHelper.WriteInfoToConsole(ConsoleVerbosity.Verbose, "- " + Rpx.Strings.Encypt_EncryptedSize, CmdHelper.GetMemStringFromBytes(encryptedBytes.Length, true), RC.Theme[ConsoleThemeColor.SubText]);

            File.WriteAllBytes(path, encryptedBytes);
        }

        public static void DecryptFile(string path, string password, uint strength)
        {
            DecryptFile(path, password, SaltValue,strength);
        }

        public static void DecryptFile(string path, string password, string saltString, uint strength)
        {
            if (!File.Exists(path))
                throw new Exception(string.Format(Rpx.Strings.Decrypt_ErrorPathDoesNotExist, path)); 

            byte[] bytes = File.ReadAllBytes(path);

            byte[] decryptedBytes = Decrypt(bytes, password, saltString, HashAlgorithm, (int)strength, InitVector, KeySize);

            File.WriteAllBytes(path, decryptedBytes);
        }

        public static byte[] Encrypt(byte[] data, string passwordString, string saltValue, 
                              string hashAlgorithm, int passwordIterations, 
                              string initVector, int keySize)
        {
            // Convert strings into byte arrays.
            // Let us assume that strings only contain ASCII codes.
            // If strings include Unicode characters, use Unicode, UTF7, or UTF8 
            // encoding.
            byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
            byte[] saltValueBytes  = Encoding.ASCII.GetBytes(saltValue);          
            
            // First, we must create a password, from which the key will be derived.
            // This password will be generated from the specified passphrase and 
            // salt value. The password will be created using the specified hash 
            // algorithm. Password creation can be done in several iterations.
            PasswordDeriveBytes password = new PasswordDeriveBytes(passwordString, saltValueBytes, hashAlgorithm, passwordIterations);
            
            // Use the password to generate pseudo-random bytes for the encryption
            // key. Specify the size of the key in bytes (instead of bits).
            byte[] keyBytes = password.GetBytes(keySize / 8);
            
            // Create uninitialized Rijndael encryption object.
            RijndaelManaged symmetricKey = new RijndaelManaged();
            
            // It is reasonable to set encryption mode to Cipher Block Chaining
            // (CBC). Use default options for other symmetric key parameters.
            symmetricKey.Mode = CipherMode.CBC;        
            
            // Generate encryptor from the existing key bytes and initialization 
            // vector. Key size will be defined based on the number of the key 
            // bytes.
            ICryptoTransform encryptor = symmetricKey.CreateEncryptor(keyBytes, initVectorBytes);
            
            // Define memory stream which will be used to hold encrypted data.
            using (MemoryStream memoryStream = new MemoryStream())
            {
                // Define cryptographic stream (always use Write mode for encryption).
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                {
                    // Start encrypting.
                    cryptoStream.Write(data, 0, data.Length);

                    // Finish encrypting.
                    cryptoStream.FlushFinalBlock();

                    // Convert our encrypted data from a memory stream into a byte array.
                    return memoryStream.ToArray();
                }
            }
        }

        public static byte[] Decrypt(byte[] cipherData, string passPhrase, string saltValue,
                                     string hashAlgorithm, int passwordIterations, 
                                     string initVector, int keySize)
        {
            // Convert strings defining encryption key characteristics into byte
            // arrays. Let us assume that strings only contain ASCII codes.
            // If strings include Unicode characters, use Unicode, UTF7, or UTF8
            // encoding.
            byte[] initVectorBytes = Encoding.ASCII.GetBytes(initVector);
            byte[] saltValueBytes  = Encoding.ASCII.GetBytes(saltValue);
            
            // First, we must create a password, from which the key will be 
            // derived. This password will be generated from the specified 
            // passphrase and salt value. The password will be created using
            // the specified hash algorithm. Password creation can be done in
            // several iterations.
            PasswordDeriveBytes password = new PasswordDeriveBytes(passPhrase, saltValueBytes, hashAlgorithm, passwordIterations);
            
            // Use the password to generate pseudo-random bytes for the encryption
            // key. Specify the size of the key in bytes (instead of bits).
            byte[] keyBytes = password.GetBytes(keySize / 8);
            
            // Create uninitialized Rijndael encryption object.
            RijndaelManaged    symmetricKey = new RijndaelManaged();
            
            // It is reasonable to set encryption mode to Cipher Block Chaining
            // (CBC). Use default options for other symmetric key parameters.
            symmetricKey.Mode = CipherMode.CBC;
            
            // Generate decryptor from the existing key bytes and initialization 
            // vector. Key size will be defined based on the number of the key 
            // bytes.
            ICryptoTransform decryptor = symmetricKey.CreateDecryptor(keyBytes, initVectorBytes);
            
            // Define memory stream which will be used to hold encrypted data.
            using (MemoryStream memoryStream = new MemoryStream(cipherData))
            {
                // Define cryptographic stream (always use Read mode for encryption).
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                {
                    // Since at this point we don't know what the size of decrypted data
                    // will be, allocate the buffer long enough to hold ciphertext;
                    // plaintext is never longer than ciphertext.
                    byte[] plainTextBytes = new byte[cipherData.Length];

                    // Start decrypting.
                    int decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);

                    byte[] finalBytes = new byte[decryptedByteCount];

                    for (int i = 0; i < decryptedByteCount; i++)
                        finalBytes[i] = plainTextBytes[i]; 

                    return finalBytes;
                }
            }           
        }
    }
}
