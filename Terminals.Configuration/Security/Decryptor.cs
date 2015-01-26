using System;
using System.IO;
using System.Security.Cryptography;

namespace Terminals.Configuration.Security
{
    public class Decryptor
    {
        private readonly DecryptTransformer transformer;

        private byte[] initVec;


        public Decryptor(EncryptionAlgorithm algId)
        {
            this.transformer = new DecryptTransformer(algId);
        }

        public byte[] IV
        {
            set { this.initVec = value; }
        }

        public byte[] Decrypt(byte[] bytesData, byte[] bytesKey)
        {
            MemoryStream memoryStream = new MemoryStream();
            this.transformer.IV = this.initVec;
            ICryptoTransform iCryptoTransform = this.transformer.GetCryptoServiceProvider(bytesKey);
            CryptoStream cryptoStream = new CryptoStream(memoryStream, iCryptoTransform, CryptoStreamMode.Write);
            try
            {
                cryptoStream.Write(bytesData, 0, bytesData.Length);
                cryptoStream.FlushFinalBlock();
                cryptoStream.Close();
                byte[] bs = memoryStream.ToArray();
                return bs;
            }
            catch (Exception e)
            {
                throw new Exception(String.Concat("Error while writing encrypted data to the stream: \n", e.Message));
            }
        }
    }
}