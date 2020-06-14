using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace Utilities
{
    public static class SecurityUtils
    {
        public static (string PublicAndPrivateKeys, string PublicKey) CreateNewRSAKeys(int keySize)
        {
            using (var rsa = new RSACryptoServiceProvider(keySize))
            {
                try
                {
                    return (rsa.ToXmlString(true), rsa.ToXmlString(false));
                }
                finally
                {
                    rsa.PersistKeyInCsp = false;
                }
            }
        }

        /// <summary>
        /// Uses <see cref="SHA512CryptoServiceProvider"/>
        /// </summary>
        /// <param name="dataToSign"></param>
        /// <param name="privateKey"></param>
        /// <returns></returns>
        public static byte[] GetSignature(byte[] dataToSign, string privateKey)
        {
            var rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(privateKey);

            return rsa.SignData(dataToSign, new SHA512CryptoServiceProvider());
        }
        /// <summary>
        /// Uses <see cref="SHA512CryptoServiceProvider"/>
        /// </summary>
        /// <param name="dataToVerify"></param>
        /// <param name="signature"></param>
        /// <param name="publicKey"></param>
        /// <returns></returns>
        public static bool VerifySignatureHash(byte[] dataToVerify, byte[] signature, string publicKey)
        {
            var rsa = new RSACryptoServiceProvider();
            rsa.FromXmlString(publicKey);

            return rsa.VerifyData(dataToVerify, new SHA512CryptoServiceProvider(), signature);
        }
    }
}
