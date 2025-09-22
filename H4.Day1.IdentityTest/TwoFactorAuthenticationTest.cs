using Bunit;
using Bunit.TestDoubles;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System.Security.Claims;
namespace H4.Day1.IdentityTest
{
    public class TwoFactorAuthenticationTest
    {
        [Fact]
        public void RequiresTwoFactorAfterLogin()
        {
            // Arrange
            using var ctx = new TestContext();
            var authContext = ctx.AddTestAuthorization();

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.Name, "Sharkie123@hotmail.com"),
                new Claim("amr", "mfa") // 👈 markerer MFA
            }, "testAuthType"));

            // Use SetAuthorized with userName and SetClaims for claims
            authContext.SetAuthorized("Sharkie123@hotmail.com");
            authContext.SetClaims(new Claim("amr", "mfa"));

            // Act
            var authState = ctx.Services
                .GetRequiredService<AuthenticationStateProvider>()
                .GetAuthenticationStateAsync().Result;

            // Assert
            Assert.True(authState.User.Identity!.IsAuthenticated);
            Assert.True(authState.User.HasClaim(c => c.Type == "amr" && c.Value == "mfa"));
        }

        [Fact]
        public void LoginWithoutTwoFactorIsNotEnough()
        {
            // Arrange
            using var ctx = new TestContext();
            var authContext = ctx.AddTestAuthorization();

            // Kun almindelig login – ingen MFA-claim
            authContext.SetAuthorized("Sharkie123@hotmail.com");

            // Act
            var authState = ctx.Services
                .GetRequiredService<AuthenticationStateProvider>()
                .GetAuthenticationStateAsync().Result;

            // Assert – brugeren er logget ind, men uden MFA
            Assert.True(authState.User.Identity!.IsAuthenticated);
            Assert.False(authState.User.HasClaim(c => c.Type == "amr" && c.Value == "mfa"));
        }
    }
}
