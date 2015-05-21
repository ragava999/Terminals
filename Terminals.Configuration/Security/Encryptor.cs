using System;
using System.IO;
using System.Security.Cryptography;

namespace Terminals.Configuration.Security
{
    public class Encryptor
    {
        private readonly EncryptTransformer transformer;
        private byte[] initVec;

        public Encryptor(EncryptionAlgorithm algId)
        {
            this.transformer = new EncryptTransformer(algId);
        }

        public byte[] IV
        {
            set { this.initVec = value; }
        }

        public byte[] Encrypt(byte[] bytesData, byte[] bytesKey)
        {
            MemoryStream memoryStream = new MemoryStream();
            this.transformer.IV = this.initVec;
            ICryptoTransform iCryptoTransform = this.transformer.GetCryptoServiceProvider(bytesKey);
            CryptoStream cryptoStream = new CryptoStream(memoryStream, iCryptoTransform, CryptoStreamMode.Write);
            try
            {
                cryptoStream.Write(bytesData, 0, bytesData.Length);
            }
            catch (Exception e)
            {
                throw new Exception(String.Concat("Error while writing encrypted data to the stream: \n", e.Message));
            }
            this.initVec = this.transformer.IV;
            cryptoStream.FlushFinalBlock();
            cryptoStream.Close();
            return memoryStream.ToArray();
        }
    }
}