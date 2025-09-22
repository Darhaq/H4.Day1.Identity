using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace H4.Day1.IdentityTest
{
    public class CertificateTest
    {
        private const string PrivateKey = @"-----BEGIN PRIVATE KEY-----
MIIEpAIBAAKCAQEA7NnoHLfEku1uzzeEABoNO6kod+/N99lCHoHpoYzcDcu9UhKlMZtGW8Ml4GDn
MVRCbeB5HT8OEQtF/y+rwZWsjwVOQ/6u52nb/eD+U+uqW7g4l4MnXtVRyin8/AN3YvujPo+hnpg0
C2m6Fw45aIcZuxBrqoIdhZNlL+cnaiTpU47Ea3RlYKWPUvx5dNSUn/Y4bFH6OMFdZZdpJRcJk916
AEN5uCi+YITdy9cudNXqc0JfvNmrGywYzoJ6kVmP3DPtx8RqOp+xS2cr7NbUwaGfXpTMAczFpIcl
zvgHvdH1p9XMAImwptoDCxeoUpIvtlGQQ5iDAjFCN2IH0zYxioBRKQIDAQABAoIBACd0QRMPlaI4
BP5lrApsTIoEqqYX/0JrIXV9hJKRqVJu/vF+A6CY0gaVImkXG0v/UjWFcAdsKoayTw2sPjs4GI+W
fawb77zCyI+o/BS0rCMtM9ghU9ybQk2f4vGEDk4hxta0DGMjzX+dNA+FIhEEeEfN+3FizVIj9HYf
VlR7I5b0XjtqS7zozOyLhrlnG3VDlzCi0TMHfNSzSwfgk8YV2eQ9Nal2grA7BT+Emo3QhM7GQuva
eoszkMuvp9sQDTYSOhkyRyDaaTWM4lo4yZU9AZjU2TB+YaH5k9rhVpnSJF6yFlldGBPBRXKu/Oop
NqNNLei3715EfXg4CI3z1MG2pqUCgYEA+56nDGcCfHaS8ID5JPjBmDXKPkltkwhXRsdFp7vADacM
BATuXKeY0ZhX7LQ4D2TX5MM/80ec/KbYx9OjiE6xQLvBkHkdac1egrW+j5J8jBCdDPnAVzD6hVlB
qWJh5TvIUuTWyiaeIXuBnG6NRcKruX44pAWYvgkrNUwTb9SnVK8CgYEA8PlwG6xDkS0djT7aaFoC
FMam1Q2Vr/SdfmEEKK6MJzF9k5sqJqihoXIWdWb3gzr9/GQZdmjKRge2vzkO97irx3z7mNgaC0ZE
scWYW13vwTYJ5VEUtMrA8m3aKjmPG2Z7/YSUK8ZXGfVY8i+2MrQ4gvzcLPm8/Jn0HQcdcL/T3acC
gYEArubaMUnNNZzqiNjt1iA/2cDSIzfBPeoXWLCdn3lnh1XtiUGwOqGZHWArBfND4Jd1ZqO96SqH
WivUFhrr2ozwsxGP/A/kPS4vGuagXoYxot7NfD6Cz1jRWy3u8YHckI5csho1n3D2jEmgj49dDffC
jH7LemAVi0suSK7n9902rysCgYB44AZ2OyDhb8oFEeQouA2XMlZ5RIkza/accDnP1k6DUnX1Vr38
ClaZT0sotO+vKiZBNkz3cnUUT8ZBgXOam02/kVf7QVs/EI5gxgc4vZQITP0FxHgogBPtB2GKELlS
O0Qy1RAmLSV/5ewp//ja0kAOpJwlC1jbNLaY4PrJlc/GvQKBgQCCQzsAat8d/BpbU5DpYkiJS4Sh
5E2BQXX6sXEb7K74/POSwljJowb0MzmSMcPQA0/tvBqgoo33EeA6SdrRShypu5N675xQ/ZX8OE0t
B/EDIFkYsYV9a3DDvLNoWsHh4Prf+caFpHjAr4ubv8sFckfLTEhQixASwuyfC7ckY1SyfA==
-----END PRIVATE KEY-----";

        private const string PublicKey = @"-----BEGIN PUBLIC KEY-----
MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEA7NnoHLfEku1uzzeEABoNO6kod+/N99lC
HoHpoYzcDcu9UhKlMZtGW8Ml4GDnMVRCbeB5HT8OEQtF/y+rwZWsjwVOQ/6u52nb/eD+U+uqW7g4
l4MnXtVRyin8/AN3YvujPo+hnpg0C2m6Fw45aIcZuxBrqoIdhZNlL+cnaiTpU47Ea3RlYKWPUvx5
dNSUn/Y4bFH6OMFdZZdpJRcJk916AEN5uCi+YITdy9cudNXqc0JfvNmrGywYzoJ6kVmP3DPtx8Rq
Op+xS2cr7NbUwaGfXpTMAczFpIclzvgHvdH1p9XMAImwptoDCxeoUpIvtlGQQ5iDAjFCN2IH0zYx
ioBRKQIDAQAB
-----END PUBLIC KEY-----";

        [Fact]
        public void CanImportPrivateKey()
        {
            using var rsa = RSA.Create();
            var privateKeyBytes = Convert.FromBase64String(
                PrivateKey.Replace("-----BEGIN PRIVATE KEY-----", "")
                          .Replace("-----END PRIVATE KEY-----", "")
                          .Replace("\n", "").Replace("\r", "").Trim());
            rsa.ImportRSAPrivateKey(privateKeyBytes, out _);

            Assert.NotNull(rsa);
            Assert.True(rsa.KeySize > 0);
        }

        [Fact]
        public void CanImportPublicKey()
        {
            using var rsa = RSA.Create();
            var publicKeyBytes = Convert.FromBase64String(
                PublicKey.Replace("-----BEGIN PUBLIC KEY-----", "")
                         .Replace("-----END PUBLIC KEY-----", "")
                         .Replace("\n", "").Replace("\r", "").Trim());
            rsa.ImportSubjectPublicKeyInfo(publicKeyBytes, out _);

            Assert.NotNull(rsa);
            Assert.True(rsa.KeySize > 0);
        }
    }
}
