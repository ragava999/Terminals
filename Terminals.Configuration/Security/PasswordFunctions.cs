using System;
using System.Security.Cryptography;
using System.Text;
using Kohl.Framework.Logging;
using Terminals.Configuration.Files.Main.Settings;

namespace Terminals.Configuration.Security
{
    public static class PasswordFunctions
    {
        private const int KEY_LENGTH = 24;
        private const int IV_LENGTH = 16;
        private static readonly EncryptionAlgorithm EncryptionAlgorithm = EncryptionAlgorithm.Rijndael;

        public static string ComputeMasterPasswordHash(string password)
        {
            return Hash.GetHash(password, Hash.HashType.SHA512);
        }

        public static string DecryptPassword(string encryptedPassword, string connectionName, bool suppressErrors = false)
        {
            return DecryptPassword(encryptedPassword, Settings.KeyMaterial, connectionName, suppressErrors);
        }

        public static string DecryptPassword(string encryptedPassword, string keyMaterial, string connectionName, bool suppressErrors = false)
        {
            try
            {
                if (String.IsNullOrEmpty(encryptedPassword))
                    return encryptedPassword;

                if (keyMaterial == string.Empty)
                    return DecryptByEmptyKey(encryptedPassword);

                return DecryptByKey(encryptedPassword, keyMaterial);
            }
            catch (Exception ex)
            {
                if (!suppressErrors)
                    Log.Error(string.Format("Unable to decrypt password for connection \"{0}\".", connectionName), ex);

                return string.Empty;
            }
        }

        private static string DecryptByKey(string encryptedPassword, string keyMaterial)
        {
            string hashedPass = keyMaterial.Substring(0, KEY_LENGTH);
            byte[] IV = Encoding.Default.GetBytes(keyMaterial.Substring(keyMaterial.Length - IV_LENGTH));
            string password = "";
            Decryptor dec = new Decryptor(EncryptionAlgorithm);
            dec.IV = IV;
            byte[] data = dec.Decrypt(Convert.FromBase64String(encryptedPassword), Encoding.Default.GetBytes(hashedPass));
            if (data != null && data.Length > 0)
            {
                password = Encoding.Default.GetString(data);
            }
            return password;
        }

        private static string DecryptByEmptyKey(string encryptedPassword)
        {
            byte[] cyphertext = Convert.FromBase64String(encryptedPassword);
            byte[] b_entropy = Encoding.UTF8.GetBytes(String.Empty);
            byte[] plaintext = ProtectedData.Unprotect(cyphertext, b_entropy, DataProtectionScope.CurrentUser);
            return Encoding.UTF8.GetString(plaintext);
        }

        public static string EncryptPassword(string decryptedPassword)
        {
            return EncryptPassword(decryptedPassword, Settings.KeyMaterial);
        }

        public static string EncryptPassword(string decryptedPassword, string keyMaterial)
        {
            try
            {
                if (String.IsNullOrEmpty(decryptedPassword))
                    return decryptedPassword;

                if (keyMaterial == string.Empty)
                    return EncryptByEmptyKey(decryptedPassword);

                return EncryptByKey(decryptedPassword, keyMaterial);
            }
            catch (Exception ex)
            {
                Log.Error("Error encrypting password", ex);
                return string.Empty;
            }
        }

        private static string EncryptByKey(string decryptedPassword, string keyMaterial)
        {
            string hashedPass = keyMaterial.Substring(0, KEY_LENGTH);
            byte[] IV = Encoding.Default.GetBytes(keyMaterial.Substring(keyMaterial.Length - IV_LENGTH));
            Encryptor enc = new Encryptor(EncryptionAlgorithm);
            enc.IV = IV;
            byte[] data = enc.Encrypt(Encoding.Default.GetBytes(decryptedPassword),
                                      Encoding.Default.GetBytes(hashedPass));
            if (data != null && data.Length > 0)
            {
                return Convert.ToBase64String(data);
            }

            return string.Empty;
        }

        private static string EncryptByEmptyKey(string decryptedPassword)
        {
            byte[] plaintext = Encoding.UTF8.GetBytes(decryptedPassword);
            byte[] b_entropy = Encoding.UTF8.GetBytes(String.Empty);
            byte[] cyphertext = ProtectedData.Protect(plaintext, b_entropy, DataProtectionScope.CurrentUser);
            return Convert.ToBase64String(cyphertext);
        }
    }
}