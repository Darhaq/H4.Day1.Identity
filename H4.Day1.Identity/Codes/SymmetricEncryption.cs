using Microsoft.AspNetCore.DataProtection;
using System.Security.Cryptography;

namespace H4.Day1.Identity.Codes
{
    public class SymmetricEncryption
    {
        private readonly IDataProtector _key;
        public SymmetricEncryption(IDataProtectionProvider key) =>
            _key = key.CreateProtector(new RSACryptoServiceProvider().ToXmlString(false));

        public string Encrypt(string textToEncrypt) =>
            _key.Protect(textToEncrypt);

        public string Decrypt(string textToDecrypt) =>
            _key.Unprotect(textToDecrypt);
    }
}
