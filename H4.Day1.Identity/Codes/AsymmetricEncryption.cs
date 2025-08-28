using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace H4.Day1.Identity.Codes
{
    public class AsymmetricEncryption
    {
        private string _publicKey;
        private string _privateKey;

        public AsymmetricEncryption()
        {
            using (RSA rsa = RSA.Create(2048))
            {
                byte[] privateKeyBytes = rsa.ExportRSAPrivateKey();
                _privateKey =
                    "-----BEGIN PRIVATE KEY-----\n" +
                    Convert.ToBase64String(privateKeyBytes, Base64FormattingOptions.InsertLineBreaks) +
                    "\n-----END PRIVATE KEY-----";

                byte[] publicKeyBytes = rsa.ExportSubjectPublicKeyInfo();
                _publicKey =
                    "-----BEGIN PUBLIC KEY-----\n" +
                    Convert.ToBase64String(publicKeyBytes, Base64FormattingOptions.InsertLineBreaks) +
                    "\n-----END PUBLIC KEY-----";
            }
        }

        public string DecryptAsymmetric(string textToDecrypt)
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                string privateKey = _privateKey
                    .Replace("-----BEGIN PRIVATE KEY-----", "")
                    .Replace("-----END PRIVATE KEY-----", "")
                    .Replace("\n", "").Replace("\r", "").Trim();

                byte[] privateKeyBytes = Convert.FromBase64String(privateKey);
                rsa.ImportRSAPrivateKey(privateKeyBytes, out _);

                byte[] byteArrayTextToDecrypt = Convert.FromBase64String(textToDecrypt);
                byte[] decryptedDataASByteArray = rsa.Decrypt(byteArrayTextToDecrypt, true);
                string decryptedDataAsString = System.Text.Encoding.UTF8.GetString(decryptedDataASByteArray);

                return decryptedDataAsString;

            }
        }

        public async Task<string> EncryptAsymmetric_webApi(string dataToEncrypt)
        {
            string? responseMessage = null;

            string[] ar = new string[2] { _publicKey, dataToEncrypt, };
            string arSerialized = Newtonsoft.Json.JsonConvert.SerializeObject(ar);
            StringContent sc = new StringContent(
                arSerialized,
                System.Text.Encoding.UTF8,
                "application/json"
            );

            using (HttpClient _httpClient = new HttpClient())
            {
                var response = await _httpClient.PostAsync("https://localhost:7182/encryptor", sc);
                responseMessage = response.Content.ReadAsStringAsync().Result;
            }

            return responseMessage;
        }
    }
}
