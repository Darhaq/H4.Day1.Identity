using Microsoft.Extensions.Configuration;
using System.Net.Http;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

namespace H4.Day1.Identity.Codes
{
    public class AsymmetricEncryption
    {
        private string _publicKey;
        private string _privateKey;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;

        public AsymmetricEncryption(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _configuration = configuration;

            string privateKeyFile = "private.key";
            string publicKeyFile = "public.key";

            if (File.Exists(privateKeyFile) && File.Exists(publicKeyFile))
            {
                // Indlæs eksisterende nøgler
                _privateKey = File.ReadAllText(privateKeyFile);
                _publicKey = File.ReadAllText(publicKeyFile);
            }
            else
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

                // Gem nøglerne i filer
                File.WriteAllText(privateKeyFile, _privateKey);
                File.WriteAllText(publicKeyFile, _publicKey);
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

        public async Task<string> EncryptAsymmetric_webApi(string dataToEncrypt, string s)
        {
            string? responseMessage = null;

            string[] ar = new string[3] { _publicKey, dataToEncrypt, s};
            string arSerialized = Newtonsoft.Json.JsonConvert.SerializeObject(ar);
            StringContent sc = new StringContent(
                arSerialized,
                System.Text.Encoding.UTF8,
                "application/json"
            );
            string testRead = _configuration.GetValue<string>("apitoken");

            _httpClient.DefaultRequestHeaders.Authorization =
                new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", testRead);

            var response = await _httpClient.PostAsync("https://localhost:7182/encryptor", sc);
            responseMessage = await response.Content.ReadAsStringAsync();

            return responseMessage;
        }

        public string GetPublicKey()
        {
            return _publicKey;
        }
    }
}
