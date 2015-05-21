using System;
using System.Security.Cryptography;

namespace Terminals.Configuration.Security
{
    internal class EncryptTransformer
    {
        private readonly EncryptionAlgorithm algorithmID;

        private byte[] encKey;
        private byte[] initVec;

        public EncryptTransformer(EncryptionAlgorithm algId)
        {
            this.algorithmID = algId;
        }


        public byte[] IV
        {
            get { return this.initVec; }

            set { this.initVec = value; }
        }

        public byte[] Key
        {
            get { return this.encKey; }
        }

        public ICryptoTransform GetCryptoServiceProvider(byte[] bytesKey)
        {
            ICryptoTransform iCryptoTransform;

            switch (this.algorithmID)
            {
                case EncryptionAlgorithm.Des:
                    DES dES = new DESCryptoServiceProvider();
                    dES.Mode = CipherMode.CBC;
                    if (bytesKey == null)
                    {
                        this.encKey = dES.Key;
                    }
                    else
                    {
                        dES.Key = bytesKey;
                        this.encKey = dES.Key;
                    }
                    if (this.initVec == null)
                    {
                        this.initVec = dES.IV;
                    }
                    else
                    {
                        dES.IV = this.initVec;
                    }
                    iCryptoTransform = dES.CreateEncryptor();
                    break;

                case EncryptionAlgorithm.TripleDes:
                    TripleDES tripleDES = new TripleDESCryptoServiceProvider();
                    tripleDES.Mode = CipherMode.CBC;
                    if (bytesKey == null)
                    {
                        this.encKey = tripleDES.Key;
                    }
                    else
                    {
                        tripleDES.Key = bytesKey;
                        this.encKey = tripleDES.Key;
                    }
                    if (this.initVec == null)
                    {
                        this.initVec = tripleDES.IV;
                    }
                    else
                    {
                        tripleDES.IV = this.initVec;
                    }
                    iCryptoTransform = tripleDES.CreateEncryptor();
                    break;

                case EncryptionAlgorithm.Rc2:
                    RC2 rC2 = new RC2CryptoServiceProvider();
                    rC2.Mode = CipherMode.CBC;
                    if (bytesKey == null)
                    {
                        this.encKey = rC2.Key;
                    }
                    else
                    {
                        rC2.Key = bytesKey;
                        this.encKey = rC2.Key;
                    }
                    if (this.initVec == null)
                    {
                        this.initVec = rC2.IV;
                    }
                    else
                    {
                        rC2.IV = this.initVec;
                    }
                    iCryptoTransform = rC2.CreateEncryptor();
                    break;

                case EncryptionAlgorithm.Rijndael:
                    Rijndael rijndael = new RijndaelManaged();
                    rijndael.Mode = CipherMode.CBC;
                    if (bytesKey == null)
                    {
                        this.encKey = rijndael.Key;
                    }
                    else
                    {
                        rijndael.Key = bytesKey;
                        this.encKey = rijndael.Key;
                    }
                    if (this.initVec == null)
                    {
                        this.initVec = rijndael.IV;
                    }
                    else
                    {
                        rijndael.IV = this.initVec;
                    }
                    iCryptoTransform = rijndael.CreateEncryptor();
                    break;

                default:
                    throw new CryptographicException(String.Concat("Algorithm ID \'", this.algorithmID,
                                                                   "\' not supported."));
            }
            return iCryptoTransform;
        }
    }
}