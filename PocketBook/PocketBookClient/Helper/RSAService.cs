
using System.Security.Cryptography;
using System.Text;

namespace PocketBookClient.Helper
{
    public  class RSAService
    {
        private static string PublicKey =>
            SecretConfiguration.GetRequired(SecretConfiguration.RsaPublicKeyKey, SecretConfiguration.RsaPublicKeyEnv);

        private static string PrivateKey =>
            SecretConfiguration.GetRequired(SecretConfiguration.RsaPrivateKeyKey, SecretConfiguration.RsaPrivateKeyEnv);

        public static void CreateKey()
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                SecretConfiguration.ApplyConfigurationValue(SecretConfiguration.RsaPublicKeyKey, rsa.ToXmlString(false));
                SecretConfiguration.ApplyConfigurationValue(SecretConfiguration.RsaPrivateKeyKey, rsa.ToXmlString(true));
            }
        }

        /// <summary>
        /// only for test
        /// </summary>
        public static void TestExample()
        {
            string originalMessage = "$ERTWESA!";
            byte[] encryptedData = EncryptData(Encoding.UTF8.GetBytes(originalMessage), PublicKey);
            string decryptedMessage = DecryptData(encryptedData, PrivateKey);

            Console.WriteLine("Original Message: " + originalMessage);
            Console.WriteLine("Encrypted Message: " + Convert.ToBase64String(encryptedData));
            Console.WriteLine("Decrypted Message: " + decryptedMessage);
        }

        public static string EncryptData(string data)
        {
            return Convert.ToBase64String(EncryptData(Encoding.UTF8.GetBytes(data), PublicKey));
        }
        public static string DecryptData(string encryptedData)
        {
            return DecryptData(Convert.FromBase64String(encryptedData), PrivateKey);
        }

        private static byte[] EncryptData(byte[] data, string publicKey)
        {
            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(publicKey);
                return rsa.Encrypt(data, false);
            }
        }

        private static string DecryptData(byte[] encryptedData, string privateKey)
        {
            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.FromXmlString(privateKey);
                var decryptedBytes = rsa.Decrypt(encryptedData, false);
                return Encoding.UTF8.GetString(decryptedBytes);
            }
        }


    }
}
