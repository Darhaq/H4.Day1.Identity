using Bunit;
using Bunit.TestDoubles;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace H4.Day1.IdentityTest
{
    public class AuthorizationTest
    {
        [Fact]
        public void AuthorizedAsAdmin()
        {
            // Arrange
            using var ctx = new TestContext();
            var authContext = ctx.AddTestAuthorization();
            //authContext.SetAuthorized("");
            authContext.SetRoles("Admin");

            // Act
            var authState = ctx.Services
                .GetRequiredService<AuthenticationStateProvider>()
                .GetAuthenticationStateAsync().Result;

            // Assert
            Assert.True(authState.User.IsInRole("Admin"));
        }

        [Fact]
        public void NotAuthorizedAsAdmin()
        {
                // Arrange
                using var ctx = new TestContext();
                var authContext = ctx.AddTestAuthorization();
                authContext.SetAuthorized("");
                //authContext.SetRoles("Admin");

                // Act
                var authState = ctx.Services
                    .GetRequiredService<AuthenticationStateProvider>()
                    .GetAuthenticationStateAsync().Result;

                // Assert
                Assert.False(authState.User.IsInRole("Admin"));
            }
    }
}
