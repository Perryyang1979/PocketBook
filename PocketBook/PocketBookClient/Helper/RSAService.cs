
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace PocketBookClient.Helper
{
    public  class RSAService
    {
        public static string publicKey = "<RSAKeyValue><Modulus>3Q0bRh7tTIRwzn5vNF6nJLqs6+X/2sB4KqR4ntkmTXa1slZhPu3pRIuwjqP+uhPgZdusVazM8tlIRg3NNyHKjh58sHsQ2NQfNWgQfdNLK38DT51DnyHUTkCc8yyrdVZmm9s44PLUhRNIGYukT/C539A7wsxAw1EtFGxiFf7Hllk=</Modulus><Exponent>AQAB</Exponent></RSAKeyValue>";
        public static string privateKey = "<RSAKeyValue><Modulus>3Q0bRh7tTIRwzn5vNF6nJLqs6+X/2sB4KqR4ntkmTXa1slZhPu3pRIuwjqP+uhPgZdusVazM8tlIRg3NNyHKjh58sHsQ2NQfNWgQfdNLK38DT51DnyHUTkCc8yyrdVZmm9s44PLUhRNIGYukT/C539A7wsxAw1EtFGxiFf7Hllk=</Modulus><Exponent>AQAB</Exponent><P>3hSdSI7Lyx2eBBaSs+PGKVbfC0mjnQVTq/yXpqEUbs4kTbNeGQM9zS7L0a+Nql7W3F/uF+OTz8LjCcPzQlvT9w==</P><Q>/tA+yiAzqmC2sDZAAFqo0Cqd0Z9rFo3hBFsOXCfLWyGP+cwdRyDBfzplPI7zWQCYBbgA4xMaKEFbmwnx+660Lw==</Q><DP>S+l8AgtduiZ7lyYoK3mWiRb60x7u4IOroESNihXRqDaqziyb46zp+TMf1KNPp4cXpujbuCrAoeoaNYJXqr087Q==</DP><DQ>VCBiqo7pH9YgWpr+lF9aHqgDRMLyMxcSdEKvwgMtY1/4Tcty2xnYaG3aIJ09gGPuzOyL9mxHFYVGfjBRYPuz5w==</DQ><InverseQ>DfkwR7AkPwwN5PGxDmdRo69NMHDZDp5kUj6c6FN9nBDZPAUO7wzELe9Q08INYaYxluPA3ohF4zLVyFCGLcRLiw==</InverseQ><D>26C5F4uE5MC2QESZAuSEl1aultd3xYX0HxI5EkLY7qBi2SVAC8FgQo20R5UjcwhqrWl8qcc9/o/qYkg2PG/jikVlsFJh3hJKL9gMMkpwYiMYISijEtuo7NhOMuPJ0CK2xVIeI5CCuPgl48Mr01Q6xvelO0C3pcxnaTiZwXLOpH0=</D></RSAKeyValue>";

        //private static string publicKey = string.Empty;
        //private static string privateKey = string.Empty;
        public static void CreateKey()
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                // Get the public and private key
                publicKey = rsa.ToXmlString(false);
                privateKey = rsa.ToXmlString(true);

                Console.WriteLine("Public Key: " + publicKey);
                Console.WriteLine("Private Key: " + privateKey);
            }
        }

        /// <summary>
        /// only for test
        /// </summary>
        public static void TestExample()
        {
            string originalMessage = "$ERTWESA!";
            byte[] encryptedData = EncryptData(Encoding.UTF8.GetBytes(originalMessage), publicKey);
            string decryptedMessage = DecryptData(encryptedData, privateKey);

            // Display the results
            Console.WriteLine("Original Message: " + originalMessage);
            Console.WriteLine("Encrypted Message: " + Convert.ToBase64String(encryptedData));
            Console.WriteLine("Decrypted Message: " + decryptedMessage);
        }

        public static string EncryptData(string data)
        {
            return Convert.ToBase64String(EncryptData(Encoding.UTF8.GetBytes(data), publicKey));
        }
        public static string DecryptData(string encryptedData)
        {
            return DecryptData(Convert.FromBase64String(encryptedData), privateKey);
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
