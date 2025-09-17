using AngleSharp;
using Bunit;
using Bunit.TestDoubles;
using H4.Day1.Identity.Codes;
using H4.Day1.Identity.Components.Pages;
using H4.Day1.Identity.Data;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace H4.Day1.IdentityTest
{
    public class AuthenticatedTest
    {
        [Fact]
        public void Aunthenticated()
        {
            // Arrange
            using var ctx = new TestContext();
            var authContext = ctx.AddTestAuthorization();
            authContext.SetAuthorized("sharkie123@hotmail.com");

            ctx.Services.AddSingleton<SymmetricEncryption>();
            ctx.Services.AddSingleton<AsymmetricEncryption>();
            ctx.Services.AddSingleton<IDataProtectionProvider, EphemeralDataProtectionProvider>();
            ctx.Services.AddSingleton<Microsoft.Extensions.Configuration.IConfiguration>(new ConfigurationBuilder().Build());

            // 👇 Registrér EF Core med InMemory provider
            ctx.Services.AddDbContext<TodoContext>(options =>
                options.UseInMemoryDatabase("TodoTestDb"));

            // Act
            var cut = ctx.RenderComponent<TodoList>();
            var indexInstance = cut.Instance;

            // Assert
            Assert.Equal("sharkie123@hotmail.com", indexInstance._userName);
        }

        [Fact]
        public void NotAunthenticated()
        {
            // Arrange
            using var ctx = new TestContext();
            var authContext = ctx.AddTestAuthorization();
            authContext.SetNotAuthorized();

            ctx.Services.AddSingleton<SymmetricEncryption>();
            ctx.Services.AddSingleton<AsymmetricEncryption>();
            ctx.Services.AddSingleton<IDataProtectionProvider, EphemeralDataProtectionProvider>();
            ctx.Services.AddSingleton<Microsoft.Extensions.Configuration.IConfiguration>(new ConfigurationBuilder().Build());

            ctx.Services.AddDbContext<TodoContext>(options =>
                options.UseInMemoryDatabase("TodoTestDb"));

            // Act
            var cut = ctx.RenderComponent<TodoList>();
            var indexInstance = cut.Instance;

            // Assert
            Assert.Equal(null, indexInstance._userName);
        }
    }
}
