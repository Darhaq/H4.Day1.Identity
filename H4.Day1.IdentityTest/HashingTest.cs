using H4.Day1.Identity.Codes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H4.Day1.IdentityTest
{
    public class HashingTest
    {
        [Fact]
        public void HashingOfPasswordAndCpr()
        {
            var password = "SuperSecret123";
            var cpr = "1234567890";

            var hashing = new HashingHandler();

            var passwordHash = hashing.SHAHashing(password); // Brug SHA256
            var cprHash = hashing.SHAHashing(cpr);

            // Check at de ikke matcher originalerne
            Assert.NotEqual(password, passwordHash);
            Assert.NotEqual(cpr, cprHash);

            // Check at hash altid giver samme type output
            Assert.False(string.IsNullOrEmpty(passwordHash));
            Assert.False(string.IsNullOrEmpty(cprHash));
        }
    }
}
