using Bunit;
using H4.Day1.Identity.Codes;
using H4.Day1.Identity.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H4.Day1.IdentityTest
{
    public class TodoFunctionalTest
    {
        [Fact]
        public void CreateReadUpdateDeleteTodo()
        {
            using var ctx = new TestContext();
            ctx.Services.AddDbContext<TodoContext>(opt => opt.UseInMemoryDatabase("TodoTestDb"));

            var db = ctx.Services.GetRequiredService<TodoContext>();

            // Først opret en Cpr-bruger
            var user = new Cpr { User = "user@test.com", CprNr = "1234567890" };
            db.Cprs.Add(user);
            db.SaveChanges();

            // CREATE
            var todo = new Todolist { Item = "Test Todo", UserId = user.Id };
            db.Todolists.Add(todo);
            db.SaveChanges();

            // READ
            var read = db.Todolists.FirstOrDefault(t => t.UserId == user.Id);
            Assert.NotNull(read);
            Assert.Equal("Test Todo", read.Item);

            // UPDATE
            read.Item = "Updated Todo";
            db.SaveChanges();
            var updated = db.Todolists.First(t => t.Int == read.Int);
            Assert.Equal("Updated Todo", updated.Item);

            // DELETE
            db.Todolists.Remove(updated);
            db.SaveChanges();
            Assert.Empty(db.Todolists.ToList());
        }

        [Fact]
        public async Task AsymmetricEncryptionTodolistAsync()
        {
            // 1. Opsæt HttpClient med BaseAddress til WebAPI
            var httpClient = new HttpClient
            {
                BaseAddress = new Uri("https://localhost:7182") // Din WebAPI skal køre her
            };

            // 2. Opsæt konfiguration med api-token
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    { "apitoken", "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJuYW1lIjoiU2hhcmtpZTEyM0Bob3RtYWlsLmNvbSIsInN1YiI6IlNoYXJraWUxMjNAaG90bWFpbC5jb20iLCJqdGkiOiJkYjQ0MmM1ZC02MTI4LTRlMTctODM3ZS1mMDQ5NGJjMDM2ZmIiLCJyb2xlIjoiQWRtaW4iLCJleHAiOjE3NTgxOTA4NTksImlzcyI6Imh0dHBzOi8vbG9jYWxob3N0OjcxODIiLCJhdWQiOiJodHRwczovL2xvY2FsaG9zdDo3MTgyIn0.MAbaVe1Q_SfUNcm3NifN4AaLb_D2z827xry2horBvZs" }
                })
                .Build();

            // 3. Opret AsymmetricEncryption instans
            var encryption = new AsymmetricEncryption(httpClient, config);

            // 4. Krypter Todo via WebAPI
            string todoPlain = "Todo via WebAPI";
            string encryptedTodo = await encryption.EncryptAsymmetric_webApi(todoPlain, "Sharkie123@hotmail.com");

            // 5. Krypter CPR via WebAPI
            string cprPlain = "1234567890";
            string encryptedCpr = await encryption.EncryptAsymmetric_webApi(cprPlain, "Sharkie123@hotmail.com");

            // 6. Tjek at WebAPI returnerede noget (ikke null/empty)
            Assert.False(string.IsNullOrWhiteSpace(encryptedTodo));
            Assert.False(string.IsNullOrWhiteSpace(encryptedCpr));

            // 7. Dekrypter lokalt med private key for Todo
            string decryptedTodo = encryption.DecryptAsymmetric(encryptedTodo);
            Assert.Equal(todoPlain, decryptedTodo);

            // 8. Dekrypter lokalt med private key for CPR
            string decryptedCpr = encryption.DecryptAsymmetric(encryptedCpr);
            Assert.Equal(cprPlain, decryptedCpr);
        }
    }
}
