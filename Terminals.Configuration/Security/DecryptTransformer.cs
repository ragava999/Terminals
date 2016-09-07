using System;
using System.Security.Cryptography;

namespace Terminals.Configuration.Security
{
    public class DecryptTransformer
    {
        private readonly EncryptionAlgorithm algorithmID;

        private byte[] initVec;


        public DecryptTransformer(EncryptionAlgorithm deCryptId)
        {
            this.algorithmID = deCryptId;
        }

        public byte[] IV
        {
            set { this.initVec = value; }
        }

        public ICryptoTransform GetCryptoServiceProvider(byte[] bytesKey)
        {
            ICryptoTransform iCryptoTransform;

            switch (this.algorithmID)
            {
                case EncryptionAlgorithm.Des:
                    DES dES = new DESCryptoServiceProvider();
                    dES.Mode = CipherMode.CBC;
                    dES.Key = bytesKey;
                    dES.IV = this.initVec;
                    iCryptoTransform = dES.CreateDecryptor();
                    break;

                case EncryptionAlgorithm.TripleDes:
                    TripleDES tripleDES = new TripleDESCryptoServiceProvider();
                    tripleDES.Mode = CipherMode.CBC;
                    iCryptoTransform = tripleDES.CreateDecryptor(bytesKey, this.initVec);
                    break;

                case EncryptionAlgorithm.Rc2:
                    RC2 rC2 = new RC2CryptoServiceProvider();
                    rC2.Mode = CipherMode.CBC;
                    iCryptoTransform = rC2.CreateDecryptor(bytesKey, this.initVec);
                    break;

                case EncryptionAlgorithm.Rijndael:
                    Rijndael rijndael = new RijndaelManaged();
                    rijndael.Mode = CipherMode.CBC;
                    iCryptoTransform = rijndael.CreateDecryptor(bytesKey, this.initVec);
                    break;

                default:
                    throw new CryptographicException(String.Concat("Algorithm ID \'", this.algorithmID,
                                                                   "\' not supported."));
            }
            return iCryptoTransform;
        }
    }
}